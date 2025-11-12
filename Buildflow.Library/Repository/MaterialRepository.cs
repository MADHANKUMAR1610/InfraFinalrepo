using Buildflow.Infrastructure.Constants;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{


    public class MaterialRepository : IMaterialRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialRepository> _logger;
        private readonly IConfiguration _config;
        private IConfiguration configuration;

        public MaterialRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<MaterialRepository> logger)
        {
            this.configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            try
            {
                // 🧩 Step 1: Get all BoQ items for the project
                var boqItems = await _context.BoqItems
                    .Include(b => b.Boq)
                    .Where(b => b.Boq != null && b.Boq.ProjectId == projectId)
                    .ToListAsync();

                // 🧩 Step 2: Get approvals for the same project
                var approvals = await _context.BoqApprovals
                    .Include(a => a.Boq)
                    .Where(a => a.Boq != null && a.Boq.ProjectId == projectId)
                    .Select(a => new
                    {
                        a.BoqId,
                        a.Boq.BoqItems,
                        a.ApprovalStatus
                    })
                    .ToListAsync();

                // 🧩 Step 3: StockInward grouped by item
                var inwardData = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId)
                    .GroupBy(x => new { x.Itemname, x.Unit })
                    .Select(g => new
                    {
                        ItemName = g.Key.Itemname,
                        Quantity = g.Sum(x => x.QuantityReceived) ?? 0,
                        Unit = g.Key.Unit
                    })
                    .ToListAsync();

                // 🧩 Step 4: StockOutward grouped by item
                var outwardData = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId)
                    .GroupBy(x => new { x.ItemName, x.Unit })
                    .Select(g => new
                    {
                        ItemName = g.Key.ItemName,
                        Quantity = g.Sum(x => x.IssuedQuantity) ?? 0,
                        Unit = g.Key.Unit
                    })
                    .ToListAsync();

                var result = new List<MaterialDto>();
                int serial = 1;

                // 🧩 Step 5: Combine all
                foreach (var boqItem in boqItems)
                {
                    var inward = inwardData.FirstOrDefault(i => i.ItemName == boqItem.ItemName);
                    var outward = outwardData.FirstOrDefault(o => o.ItemName == boqItem.ItemName);
                    var approval = approvals.FirstOrDefault(a => a.BoqItems.Any(b => b.ItemName == boqItem.ItemName));

                    decimal inwardQty = inward?.Quantity ?? 0;
                    decimal outwardQty = outward?.Quantity ?? 0;
                    decimal inStockQty = inwardQty - outwardQty;
                    if (inStockQty < 0) inStockQty = 0;

                    // Required quantity from hardcoded requirement (if available)
                    DailyStockRequirement.RequiredStock.TryGetValue(boqItem.ItemName ?? "", out decimal requiredStock);
                    decimal requiredQty = requiredStock > 0 ? requiredStock - outwardQty : boqItem.Quantity ?? 0;
                    if (requiredQty < 0) requiredQty = 0;

                    string unit = boqItem.Unit ?? inward?.Unit ?? outward?.Unit ?? "Units";

                    // Level logic
                    string level =
                        (inStockQty <= requiredQty * 0.3m) ? "Urgent" :
                        (inStockQty <= requiredQty * 0.6m) ? "High" :
                        (inStockQty <= requiredQty * 0.9m) ? "Medium" : "Low";

                    // Request Status
                    string requestStatus = approval?.ApprovalStatus ?? "Pending";

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = boqItem.ItemName ?? "Unknown",
                        InStockQuantity = $"{inStockQty} {unit}",
                        RequiredQuantity = $"{requiredQty} {unit}",
                        Level = level,
                        RequestStatus = requestStatus,

                    });
                }

                // Also include any items only in the hardcoded list (not in BoQ)
                foreach (var hardcoded in DailyStockRequirement.RequiredStock)
                {
                    if (!result.Any(r => r.MaterialList == hardcoded.Key))
                    {
                        var inward = inwardData.FirstOrDefault(i => i.ItemName == hardcoded.Key);
                        var outward = outwardData.FirstOrDefault(o => o.ItemName == hardcoded.Key);

                        decimal inwardQty = inward?.Quantity ?? 0;
                        decimal outwardQty = outward?.Quantity ?? 0;
                        decimal inStockQty = inwardQty - outwardQty;
                        if (inStockQty < 0) inStockQty = 0;

                        decimal requiredQty = hardcoded.Value - outwardQty;
                        if (requiredQty < 0) requiredQty = 0;

                        string unit = inward?.Unit ?? outward?.Unit ?? "Units";

                        string level =
                            (inStockQty <= hardcoded.Value * 0.3m) ? "Urgent" :
                            (inStockQty <= hardcoded.Value * 0.6m) ? "High" :
                            (inStockQty <= hardcoded.Value * 0.9m) ? "Medium" : "Low";

                        result.Add(new MaterialDto
                        {
                            SNo = serial++,
                            MaterialList = hardcoded.Key,
                            InStockQuantity = $"{inStockQty} {unit}",
                            RequiredQuantity = $"{requiredQty} {unit}",
                            Level = level,
                            RequestStatus = "Pending",

                        });
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MaterialRepository.GetMaterialStatusAsync");
                throw;
            }
        }
    }
}