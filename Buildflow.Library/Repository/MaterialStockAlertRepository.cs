using Buildflow.Infrastructure.Constants;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class MaterialStockAlertRepository : IMaterialStockAlertRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialStockAlertRepository> _logger;
        private readonly IConfiguration _configuration;

        public MaterialStockAlertRepository(
            IConfiguration configuration,
            BuildflowAppContext context,
            ILogger<MaterialStockAlertRepository> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<List<MaterialDto>> GetMaterialStockAlertsAsync(int projectId)
        {
            try
            {
                // 1) BOQ items for project
                var boqItems = await _context.BoqItems
                    .Include(b => b.Boq)
                    .Where(b => b.Boq != null && b.Boq.ProjectId == projectId)
                    .ToListAsync();

                // 2) Stock inward grouped
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

                // 3) Stock outward grouped
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

                // 4) Process BOQ items
                foreach (var boqItem in boqItems)
                {
                    // Lookup hardcoded required (case-insensitive)
                    var match = DailyStockRequirement.RequiredStock
                        .FirstOrDefault(x =>
                            x.Key.Equals(boqItem.ItemName ?? "", StringComparison.OrdinalIgnoreCase));

                    decimal hardcodedRequired = match.Value;
                    if (hardcodedRequired == 0)
                        continue; // skip items not in the hardcoded list

                    var inward = inwardData.FirstOrDefault(i => i.ItemName.Equals(boqItem.ItemName, StringComparison.OrdinalIgnoreCase));
                    var outward = outwardData.FirstOrDefault(o => o.ItemName.Equals(boqItem.ItemName, StringComparison.OrdinalIgnoreCase));

                    decimal inwardQty = inward?.Quantity ?? 0;
                    decimal outwardQty = outward?.Quantity ?? 0;

                    // In-stock = inward - outward
                    decimal inStockQty = inwardQty - outwardQty;
                    if (inStockQty < 0) inStockQty = 0;

                    // Required quantity as you requested: hardcoded - outward
                    decimal requiredQty = hardcodedRequired - outwardQty;
                    if (requiredQty < 0) requiredQty = 0;

                    // URGENT condition: InStock <= Required / 3 (1:3)
                    bool isUrgent = (requiredQty > 0) && (inStockQty <= requiredQty / 3m);
                    if (!isUrgent)
                        continue; // skip non-urgent

                    string unit = boqItem.Unit ?? inward?.Unit ?? outward?.Unit ?? "Units";

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = boqItem.ItemName ?? "Unknown",
                        InStockQuantity = $"{inStockQty:N2} {unit}",
                        RequiredQuantity = $"{requiredQty:N2} {unit}",
                        Level = "Urgent",
                        RequestStatus = "Pending"
                    });
                }

                // 5) Include hardcoded-only items (not in BOQ)
                foreach (var hardcoded in DailyStockRequirement.RequiredStock)
                {
                    // if already added (case-insensitive), skip
                    if (result.Any(r => r.MaterialList.Equals(hardcoded.Key, StringComparison.OrdinalIgnoreCase)))
                        continue;

                    var inward = inwardData.FirstOrDefault(i => i.ItemName.Equals(hardcoded.Key, StringComparison.OrdinalIgnoreCase));
                    var outward = outwardData.FirstOrDefault(o => o.ItemName.Equals(hardcoded.Key, StringComparison.OrdinalIgnoreCase));

                    decimal inwardQty = inward?.Quantity ?? 0;
                    decimal outwardQty = outward?.Quantity ?? 0;

                    decimal inStockQty = inwardQty - outwardQty;
                    if (inStockQty < 0) inStockQty = 0;

                    decimal requiredQty = hardcoded.Value - outwardQty;
                    if (requiredQty < 0) requiredQty = 0;

                    bool isUrgent = (requiredQty > 0) && (inStockQty <= requiredQty / 3m);
                    if (!isUrgent)
                        continue;

                    string unit = inward?.Unit ?? outward?.Unit ?? "Units";

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = hardcoded.Key,
                        InStockQuantity = $"{inStockQty:N2} {unit}",
                        RequiredQuantity = $"{requiredQty:N2} {unit}",
                        Level = "Urgent",
                        RequestStatus = "Pending"
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching MaterialStockAlerts");
                throw;
            }
        }
    }
}
