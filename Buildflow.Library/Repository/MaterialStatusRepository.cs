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
    public class MaterialStatusRepository : IMaterialStatusRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialStatusRepository> _logger;
        private readonly IConfiguration _configuration;

        public MaterialStatusRepository(
            IConfiguration configuration,
            BuildflowAppContext context,
            ILogger<MaterialStatusRepository> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                // 1️⃣ Load today's DailyStock (create with carry-forward if missing)
                var todayStock = await _context.DailyStocks
                    .Where(x => x.Date.Date == today)
                    .ToListAsync();

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
                            RemainingQty = kvp.Value + carryForward, // ✅ carry-forward
                            Date = today
                        });
                    }

                    await _context.DailyStocks.AddRangeAsync(newStock);
                    await _context.SaveChangesAsync();
                    todayStock = newStock;
                }

                // 2️⃣ Get total inward/outward grouped data
                var inwardGroups = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId)
                    .GroupBy(x => new { x.Itemname, x.Unit })
                    .Select(g => new
                    {
                        ItemName = g.Key.Itemname,
                        Unit = g.Key.Unit ?? string.Empty,
                        Quantity = g.Sum(x => x.QuantityReceived) ?? 0
                    })
                    .ToListAsync();

                var outwardGroups = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId)
                    .GroupBy(x => new { x.ItemName, x.Unit })
                    .Select(g => new
                    {
                        ItemName = g.Key.ItemName,
                        Unit = g.Key.Unit ?? string.Empty,
                        Quantity = g.Sum(x => x.IssuedQuantity) ?? 0
                    })
                    .ToListAsync();

                var materials = new List<MaterialStatusDto>();

                // 3️⃣ Compute required values for each item
                foreach (var reqItem in DailyStockRequirement.RequiredStock)
                {
                    string itemName = reqItem.Key;
                    decimal dailyRequirement = reqItem.Value;

                    var inwardItem = inwardGroups.FirstOrDefault(x => x.ItemName == itemName);
                    var outwardItem = outwardGroups.FirstOrDefault(x => x.ItemName == itemName);
                    var stock = todayStock.FirstOrDefault(s => s.ItemName == itemName);

                    decimal inwardQty = inwardItem?.Quantity ?? 0;
                    decimal outwardQty = outwardItem?.Quantity ?? 0;
                    string unit = inwardItem?.Unit ?? outwardItem?.Unit ?? "Units";

                    // 🧮 InStock = TotalInward - TotalOutward
                    decimal inStock = inwardQty - outwardQty;
                    if (inStock < 0) inStock = 0;

                    // 🧮 BalanceToGiveToday = (DailyRequirement + CarryForward) - OutwardToday
                    decimal carryForward = (stock?.RemainingQty ?? 0) - dailyRequirement;
                    if (carryForward < 0) carryForward = 0;

                    decimal balanceToGive = dailyRequirement + carryForward - outwardQty;
                    if (balanceToGive < 0) balanceToGive = 0;

                    // 🧮 RequiredToBuy = BalanceToGive - InStock
                    decimal requiredQty = balanceToGive - inStock;
                    if (requiredQty < 0) requiredQty = 0;

                    // 🔁 Update today's carry-forward (RemainingQty)
                    stock.RemainingQty = requiredQty;
                    _context.DailyStocks.Update(stock);

                    materials.Add(new MaterialStatusDto
                    {
                        ItemName = itemName,
                        InStockDisplay = $"{inStock:N2} {unit}",
                        RequiredDisplay = $"{requiredQty:N2} {unit}"
                    });
                }

                await _context.SaveChangesAsync();
                return materials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MaterialStatusRepository.GetMaterialStatusAsync");
                throw;
            }
        }
    }
}
