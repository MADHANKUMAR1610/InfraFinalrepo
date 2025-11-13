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
            IDailyStockRepository dailyStockRepository)  // add this
        {
            _config = configuration;
            _context = context;
            _logger = logger;
            _dailyStockRepository = dailyStockRepository; // assign
        }

        // ✅ Called from frontend or dashboard to get material details
        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);
            var materials = new List<MaterialDto>();
            int serial = 1;

            foreach (var material in DailyStockRequirement.RequiredStock)
            {
                string itemName = material.Key;
                decimal todayHardcoded = material.Value;

                // ------------------ Fetch inward/outward ------------------
                decimal yesterdayInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId && x.Itemname == itemName && x.DateReceived.Value.Date == yesterday)
                    .SumAsync(x => (decimal?)x.QuantityReceived) ?? 0;

                decimal yesterdayOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId && x.ItemName == itemName && x.DateIssued.Value.Date == yesterday)
                    .SumAsync(x => (decimal?)x.IssuedQuantity) ?? 0;

                decimal todayInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId && x.Itemname == itemName && x.DateReceived.Value.Date == today)
                    .SumAsync(x => (decimal?)x.QuantityReceived) ?? 0;

                decimal todayOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId && x.ItemName == itemName && x.DateIssued.Value.Date == today)
                    .SumAsync(x => (decimal?)x.IssuedQuantity) ?? 0;

                // ------------------ Yesterday's Remaining ------------------
                var yesterdayStock = await _context.DailyStocks
                    .FirstOrDefaultAsync(d => d.ProjectId == projectId && d.ItemName == itemName && d.Date == yesterday);
                decimal yesterdayRemaining = yesterdayStock?.RemainingQty ?? 0;

                // ------------------ Calculate Logic ------------------
                // ✅ InStock = (YesterdayInward - YesterdayOutward) + (TodayInward - TodayOutward)
                decimal inStock = (yesterdayInward - yesterdayOutward) + (todayInward - todayOutward);
                if (inStock < 0) inStock = 0;

                // ✅ RequiredQty = Yesterday’s Remaining + Today’s Hardcoded − Today’s Outward
                decimal requiredQty = yesterdayRemaining + todayHardcoded - todayOutward;
                if (requiredQty < 0) requiredQty = 0;

                // ------------------ Store/Update DailyStock ------------------
                await UpdateOrInsertDailyStockAsync(projectId, itemName, todayHardcoded, requiredQty, inStock);

                // ------------------ Prepare MaterialDto ------------------
                materials.Add(new MaterialDto
                {
                    SNo = serial++,
                    MaterialList = itemName,
                    InStockQuantity = $"{inStock} Units",
                    RequiredQuantity = $"{requiredQty} Units",
                    Level =
                        (inStock <= todayHardcoded / 3) ? "Urgent" :
                        (inStock <= todayHardcoded * 0.6m) ? "High" :
                        (inStock <= todayHardcoded * 0.9m) ? "Medium" : "Low",
                    RequestStatus = "Pending"
                });
            }

            return materials;
        }

        // ✅ Auto-updates DailyStock table when recalculated or new day starts
        private async Task UpdateOrInsertDailyStockAsync(int projectId, string itemName, decimal defaultQty, decimal requiredQty, decimal inStock)
        {
            var today = DateTime.UtcNow.Date;
            var todayStock = await _context.DailyStocks
                .FirstOrDefaultAsync(d => d.ProjectId == projectId && d.ItemName == itemName && d.Date == today);

            if (todayStock == null)
            {
                todayStock = new DailyStock
                {
                    ProjectId = projectId,
                    ItemName = itemName,
                    DefaultQty = defaultQty,
                 
                    RemainingQty = requiredQty,
                    Date = today
                };
                await _context.DailyStocks.AddAsync(todayStock);
            }
            else
            {
                todayStock.DefaultQty = defaultQty;
                todayStock.RemainingQty = requiredQty;
                
                _context.DailyStocks.Update(todayStock);
            }

            await _context.SaveChangesAsync();
        }

        // ✅ Recalculate only when new stock movement or new day starts
        public async Task TriggerRecalculationIfNeededAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            bool inwardExists = await _context.StockInwards.AnyAsync(x => x.ProjectId == projectId && x.DateReceived.Value.Date == today);
            bool outwardExists = await _context.StockOutwards.AnyAsync(x => x.ProjectId == projectId && x.DateIssued.Value.Date == today);
            bool todayStockExists = await _context.DailyStocks.AnyAsync(x => x.ProjectId == projectId && x.Date == today);

            if (!todayStockExists || inwardExists || outwardExists)
            {
                await GetMaterialAsync(projectId);
            }
        }
    }
}
