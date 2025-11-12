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
    public class MaterialRepository : IMaterialRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialRepository> _logger;
        private readonly IConfiguration _config;
        private readonly IDailyStockRepository _dailyStockRepository;

        public MaterialRepository(
            IConfiguration configuration,
            BuildflowAppContext context,
            ILogger<MaterialRepository> logger,
            IDailyStockRepository dailyStockRepository)
        {
            _config = configuration;
            _context = context;
            _logger = logger;
            _dailyStockRepository = dailyStockRepository;
        }

        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;

                // 🧩 Step 1: Get existing DailyStock for today
                var todayStock = await _context.DailyStocks
                    .Where(x => x.Date.Date == today)
                    .ToListAsync();

                // 🧩 Step 2: If today’s stock doesn’t exist, carry forward yesterday’s
                if (!todayStock.Any())
                {
                    var yesterday = today.AddDays(-1);
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
                            RemainingQty = kvp.Value + carryForward, // 2000 + yesterday's leftover
                            Date = today
                        });
                    }

                    await _context.DailyStocks.AddRangeAsync(newStock);
                    await _context.SaveChangesAsync();

                    todayStock = newStock;
                }

                // 🧩 Step 3: Get Inward and Outward data
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

                // 🧩 Step 4: Combine logic for each material
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

                    // ✅ Daily Requirement Logic (what you need today)
                    // If you required 2000/day and only used 500 today → 1500 carried forward
                    decimal todayRequired = stock?.RemainingQty ?? requiredPerDay;
                    decimal balanceToday = todayRequired - outwardQty;
                    if (balanceToday < 0) balanceToday = 0;

                    // ✅ Update DailyStock.RemainingQty for this item
                    stock.RemainingQty = balanceToday;
                    _context.DailyStocks.Update(stock);

                    // ✅ Level Calculation (same as before)
                    string level =
                        (inStockQty <= requiredPerDay / 3) ? "Urgent" :
                        (inStockQty <= requiredPerDay * 0.6m) ? "High" :
                        (inStockQty <= requiredPerDay * 0.9m) ? "Medium" : "Low";

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = itemName,
                        InStockQuantity = $"{inStockQty} {(inward?.Unit ?? outward?.Unit ?? "Units")}",
                        RequiredQuantity = $"{balanceToday} {(inward?.Unit ?? outward?.Unit ?? "Units")}",
                        Level = level,
                        RequestStatus = "Pending"
                    });
                }

                await _context.SaveChangesAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MaterialRepository.GetMaterialAsync");
                throw;
            }
        }
    }
}
