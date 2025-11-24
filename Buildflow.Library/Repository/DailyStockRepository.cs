using Buildflow.Infrastructure.Constants;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class DailyStockRepository : IDailyStockRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<DailyStockRepository> _logger;

        public DailyStockRepository(
            
            BuildflowAppContext context,
            ILogger<DailyStockRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        // --------------------------------------------------------------------
        // 1️⃣ RESET DAILY STOCK (NEW DAY)
        // --------------------------------------------------------------------
        public async Task ResetDailyStockAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            // If today's rows already exist → skip
            if (await _context.DailyStocks.AnyAsync(d =>
                d.ProjectId == projectId && d.Date == today))
                return;

            // Yesterday rows
            var yesterdayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == yesterday)
                .ToListAsync();

            var newStocks = new List<DailyStock>();

            foreach (var item in DailyStockRequirement.RequiredStock)
            {
                string itemName = item.Key;
                decimal todayHardcoded = item.Value;

                decimal yesterdayRemaining =
                    yesterdayStocks.FirstOrDefault(x => x.ItemName == itemName)?.RemainingQty ?? 0;

                // Total Inward/Outward up to today
                decimal totalInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId && x.Itemname == itemName)
                    .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                decimal totalOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId && x.ItemName == itemName)
                    .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                decimal inStock = totalInward - totalOutward;
                if (inStock < 0) inStock = 0;

                // REQUIRED FORMULA WITHOUT BOQ (BOQ will be applied separately)
                decimal requiredQty =
                    (yesterdayRemaining + todayHardcoded)
                    - totalOutward
                    - inStock;

                if (requiredQty < 0)
                    requiredQty = 0;

                newStocks.Add(new DailyStock
                {
                    ProjectId = projectId,
                    ItemName = itemName,
                    DefaultQty = todayHardcoded,
                    RemainingQty = requiredQty,
                    InStock = inStock,
                    Date = today
                });
            }

            await _context.DailyStocks.AddRangeAsync(newStocks);
            await _context.SaveChangesAsync();
        }



        // --------------------------------------------------------------------
        // 2️⃣ APPLY BOQ QUANTITIES ON THE DAY OF BOQ CREATION (OPTION-B)
        // --------------------------------------------------------------------
        public async Task ApplyBoqItemsToDailyStockAsync(int projectId, List<BoqItem> boqItems)
        {
            var today = DateTime.UtcNow.Date;

            // make sure today’s stock rows exist
            await ResetDailyStockAsync(projectId);

            foreach (var item in boqItems)
            {
                if (item.ItemName == null || item.Quantity == null)
                    continue;

                string itemName = item.ItemName;
                decimal boqQty = item.Quantity.Value;

                var todayStock = await _context.DailyStocks
                    .FirstOrDefaultAsync(d => d.ProjectId == projectId &&
                                              d.ItemName == itemName &&
                                              d.Date == today);

                if (todayStock == null)
                    continue;

                // Add BOQ qty to today's RequiredQty BEFORE final calculation adjustment
                todayStock.RemainingQty += boqQty;

                await RecalculateRequiredQty(projectId, itemName);
            }

            await _context.SaveChangesAsync();
        }



        // --------------------------------------------------------------------
        // 3️⃣ INTERNAL METHOD TO APPLY FINAL FORMULA
        // --------------------------------------------------------------------
        private async Task RecalculateRequiredQty(int projectId, string itemName)
        {
            var today = DateTime.UtcNow.Date;

            var todayStock = await _context.DailyStocks
                .FirstOrDefaultAsync(x => x.ProjectId == projectId &&
                                          x.ItemName == itemName &&
                                          x.Date == today);

            if (todayStock == null)
                return;

            // Recalculate totals for today
            decimal totalInward = await _context.StockInwards
                .Where(x => x.ProjectId == projectId && x.Itemname == itemName)
                .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

            decimal totalOutward = await _context.StockOutwards
                .Where(x => x.ProjectId == projectId && x.ItemName == itemName)
                .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

            decimal inStock = totalInward - totalOutward;
            if (inStock < 0) inStock = 0;

            // FINAL REQUIRED FORMULA
            decimal requiredQty =
                todayStock.RemainingQty  // (this already contains yesterday + hardcoded + BOQ)
                - totalOutward
                - inStock;

            if (requiredQty < 0)
                requiredQty = 0;

            todayStock.InStock = inStock;
            todayStock.RemainingQty = requiredQty;

            await _context.SaveChangesAsync();
        }



        // --------------------------------------------------------------------
        // 4️⃣ UPDATE ON INWARD/OUTWARD
        // --------------------------------------------------------------------
        public async Task UpdateDailyStockAsync(int projectId, string itemName, decimal outwardQty = 0, decimal inwardQty = 0)
        {
            var today = DateTime.UtcNow.Date;

            // Find today's DailyStock row for this item
            var stock = await _context.DailyStocks
                .FirstOrDefaultAsync(d =>
                    d.ProjectId == projectId &&
                    d.ItemName == itemName &&
                    d.Date == today);

            // If no row exists, create one with default values
            if (stock == null)
            {
                stock = new DailyStock
                {
                    ProjectId = projectId,
                    ItemName = itemName,
                    DefaultQty = 0,
                    RemainingQty = 0,
                    InStock = 0,
                    Date = today
                };

                await _context.DailyStocks.AddAsync(stock);
            }

            // Fetch totals from DB
            decimal totalInward = await _context.StockInwards
                .Where(x => x.ProjectId == projectId && x.Itemname == itemName)
                .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

            decimal totalOutward = await _context.StockOutwards
                .Where(x => x.ProjectId == projectId && x.ItemName == itemName)
                .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

            // InStock calculation
            stock.InStock = totalInward - totalOutward;
            if (stock.InStock < 0)
                stock.InStock = 0;

            // NEW RemainingQty Formula
            // Remaining BOQ qty = RemainingQty - outward + inward
            stock.RemainingQty = stock.RemainingQty - outwardQty + inwardQty;

            if (stock.RemainingQty < 0)
                stock.RemainingQty = 0;

            await _context.SaveChangesAsync();
        }


        // --------------------------------------------------------------------
        // 5️⃣ BULK UPDATE
        // --------------------------------------------------------------------
        public async Task UpdateDailyStockForProjectAsync(int projectId)
        {
            var items = await _context.StockInwards
                .Where(x => x.ProjectId == projectId)
                .Select(x => x.Itemname)
                .Union(
                    _context.StockOutwards
                        .Where(x => x.ProjectId == projectId)
                        .Select(x => x.ItemName))
                .Distinct()
                .ToListAsync();

            foreach (var item in items)
                await RecalculateRequiredQty(projectId, item);
        }



        // --------------------------------------------------------------------
        // 6️⃣ GET DAILY STOCK
        // --------------------------------------------------------------------
        public async Task<List<(string ItemName, decimal RemainingQty)>> GetDailyStockAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;

            return await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == today)
                .Select(d => new ValueTuple<string, decimal>(d.ItemName, d.RemainingQty))
                .ToListAsync();
        }


        public async Task AddNewBoqItemsToDailyStockAsync(int projectId, int boqId)
        {
            var today = DateTime.UtcNow.Date;

            var boqItems = await _context.BoqItems
                .Where(b => b.BoqId == boqId)
                .ToListAsync();

            foreach (var item in boqItems)
            {
                if (string.IsNullOrWhiteSpace(item.ItemName))
                    continue;

                var existing = await _context.DailyStocks
                    .FirstOrDefaultAsync(d =>
                        d.ProjectId == projectId &&
                        d.ItemName == item.ItemName &&
                        d.Date == today);

                if (existing == null)
                {
                    // Create initial entry from BOQ
                    await _context.DailyStocks.AddAsync(new DailyStock
                    {
                        ProjectId = projectId,
                        ItemName = item.ItemName!,
                        DefaultQty = 0,
                        RemainingQty = item.Quantity ?? 0, // BOQ REQUIRED QTY
                        InStock = 0,
                        Date = today
                    });
                }
                else
                {
                    // If BOQ updated, update remaining qty
                    existing.RemainingQty = item.Quantity ?? 0;
                }
            }

            await _context.SaveChangesAsync();
        }
        public async Task SaveCalculatedStockAsync(int projectId, string itemName, decimal instock, decimal required)
        {
            var today = DateTime.UtcNow.Date;

            var stock = await _context.DailyStocks
                .FirstOrDefaultAsync(x =>
                    x.ProjectId == projectId &&
                    x.ItemName == itemName &&
                    x.Date == today);

            if (stock == null)
            {
                stock = new DailyStock
                {
                    ProjectId = projectId,
                    ItemName = itemName,
                    Date = today,
                    DefaultQty = 0
                };

                await _context.DailyStocks.AddAsync(stock);
            }

            stock.InStock = instock;
            stock.RemainingQty = required;

            await _context.SaveChangesAsync();
        }




    }
}
