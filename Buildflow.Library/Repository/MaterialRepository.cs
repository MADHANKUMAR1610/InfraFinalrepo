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
                var yesterday = today.AddDays(-1);

                var result = new List<MaterialDto>();
                int serial = 1;

                foreach (var required in DailyStockRequirement.RequiredStock)
                {
                    string itemName = required.Key;
                    decimal dailyHardcodedQty = required.Value;

                    // Step 1: Get total inward and outward for this item
                    decimal totalInward = await _context.StockInwards
                        .Where(x => x.ProjectId == projectId && x.Itemname == itemName)
                        .SumAsync(x => (decimal?)x.QuantityReceived) ?? 0;

                    decimal totalOutward = await _context.StockOutwards
                        .Where(x => x.ProjectId == projectId && x.ItemName == itemName)
                        .SumAsync(x => (decimal?)x.IssuedQuantity) ?? 0;

                    // Step 2: In-stock quantity = total inward - total outward
                    decimal inStockQty = totalInward - totalOutward;
                    if (inStockQty < 0) inStockQty = 0;

                    // Step 3: Ensure today's stock exists
                    await UpdateDailyStockAsync(itemName, 0); // qtyChange=0 to create stock if missing
                    var todayStock = await _context.DailyStocks
                        .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date == today);

                    // Step 4: Yesterday's leftover
                    var yesterdayStock = await _context.DailyStocks
                        .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date == yesterday);
                    decimal leftoverFromYesterday = yesterdayStock?.RemainingQty ?? 0;

                    // Step 5: Required quantity = yesterday leftover + today's hardcoded
                    decimal requiredQty = leftoverFromYesterday + dailyHardcodedQty;

                    // Step 6: Update today's stock RemainingQty
                    todayStock.RemainingQty = requiredQty;
                    _context.DailyStocks.Update(todayStock);

                    // Step 7: Level calculation based on daily hardcoded only
                    string level =
                        (inStockQty <= dailyHardcodedQty / 3) ? "Urgent" :
                        (inStockQty <= dailyHardcodedQty * 0.6m) ? "High" :
                        (inStockQty <= dailyHardcodedQty * 0.9m) ? "Medium" : "Low";

                    // Step 8: Add to result DTO
                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = itemName,
                        InStockQuantity = $"{inStockQty} Units",
                        RequiredQuantity = $"{requiredQty} Units",
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

        // Helper to create/update daily stock when inward/outward happens or for new day
        private async Task UpdateDailyStockAsync(string itemName, decimal qtyChange)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var yesterdayStock = await _context.DailyStocks
                .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date == yesterday);

            var todayStock = await _context.DailyStocks
                .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date == today);

            decimal carryForward = yesterdayStock?.RemainingQty ?? 0;

            if (todayStock == null)
            {
                todayStock = new DailyStock
                {
                    ItemName = itemName,
                    DefaultQty = carryForward,
                    RemainingQty = carryForward + qtyChange,
                    Date = today
                };
                await _context.DailyStocks.AddAsync(todayStock);
            }
            else
            {
                todayStock.RemainingQty += qtyChange;
                if (todayStock.RemainingQty < 0) todayStock.RemainingQty = 0;
            }

            await _context.SaveChangesAsync();
        }
    }
}
