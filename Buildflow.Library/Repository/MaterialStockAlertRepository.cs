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
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                // 1️⃣ Get today's DailyStock
                var todayStock = await _context.DailyStocks
                    .Where(x => x.Date.Date == today)
                    .ToListAsync();

                // 2️⃣ If not exists, carry forward from yesterday + hardcoded
                if (!todayStock.Any())
                {
                    var yesterdayStock = await _context.DailyStocks
                        .Where(x => x.Date.Date == yesterday)
                        .ToListAsync();

                    var newStock = new List<DailyStock>();

                    foreach (var kvp in DailyStockRequirement.RequiredStock)
                    {
                        var previous = yesterdayStock.FirstOrDefault(x => x.ItemName == kvp.Key);
                        decimal carryForward = previous?.RemainingQty ?? 0;
                        newStock.Add(new DailyStock
                        {
                            ItemName = kvp.Key,
                            DefaultQty = kvp.Value,
                            RemainingQty = kvp.Value + carryForward, // 2000 + yesterday’s leftover
                            Date = today
                        });
                    }

                    await _context.DailyStocks.AddRangeAsync(newStock);
                    await _context.SaveChangesAsync();

                    todayStock = newStock;
                }

                // 3️⃣ Inward and Outward grouped data
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

                // 4️⃣ Process each item (BoQ + Hardcoded)
                foreach (var required in DailyStockRequirement.RequiredStock)
                {
                    var itemName = required.Key;
                    var requiredPerDay = required.Value;

                    var inward = inwardData.FirstOrDefault(i => i.ItemName == itemName);
                    var outward = outwardData.FirstOrDefault(o => o.ItemName == itemName);
                    var stock = todayStock.FirstOrDefault(s => s.ItemName == itemName);

                    decimal inwardQty = inward?.Quantity ?? 0;
                    decimal outwardQty = outward?.Quantity ?? 0;

                    // ✅ InStock = Total Inward - Total Outward
                    decimal inStockQty = inwardQty - outwardQty;
                    if (inStockQty < 0) inStockQty = 0;

                    // ✅ Today's Required = (Hardcoded + PreviousBalance) - Outward
                    decimal todayRequired = stock?.RemainingQty ?? requiredPerDay;
                    decimal requiredQty = todayRequired - outwardQty;
                    if (requiredQty < 0) requiredQty = 0;

                    // ✅ Update today's RemainingQty (carry-forward balance)
                    stock.RemainingQty = requiredQty;
                    _context.DailyStocks.Update(stock);

                    // ✅ Check Urgent (1:3 ratio)
                    bool isUrgent = requiredQty > 0 && inStockQty <= (requiredQty / 3m);
                    if (!isUrgent)
                        continue;

                    string unit = inward?.Unit ?? outward?.Unit ?? "Units";

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = itemName,
                        InStockQuantity = $"{inStockQty:N2} {unit}",
                        RequiredQuantity = $"{requiredQty:N2} {unit}",
                        Level = "Urgent",
                        RequestStatus = "Pending"
                    });
                }

                await _context.SaveChangesAsync();
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
