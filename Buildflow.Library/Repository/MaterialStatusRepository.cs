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
        private readonly IDailyStockRepository _dailyStockRepository;

        private readonly ILogger<MaterialStatusRepository> _logger;
        private readonly IConfiguration _configuration;

        public MaterialStatusRepository(
            IConfiguration configuration,
            BuildflowAppContext context,
            ILogger<MaterialStatusRepository> logger,
             IDailyStockRepository dailyStockRepository)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _dailyStockRepository = dailyStockRepository;
        }

      

        // ✅ Get material status for display (calculates dynamically, no DB writes)
        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;


                // Get all items linked to this project from inward & outward
                var dbItems = await _context.StockInwards
                  .Where(x => x.ProjectId == projectId)
                  .Select(x => x.Itemname)
                  .Union(
                      _context.StockOutwards
                      .Where(x => x.ProjectId == projectId)
                      .Select(x => x.ItemName)
                  )
                  .Distinct()
                  .ToListAsync();

                var allItems = DailyStockRequirement.RequiredStock.Keys
                    .Union(dbItems)   // combine both hardcoded + dynamic
                    .Distinct()
                    .ToList();


                var materials = new List<MaterialStatusDto>();

                foreach (var itemName in allItems)
                {
                    var dto = await CalculateMaterialStatusAsync(projectId, itemName);
                    materials.Add(dto);
                    _logger.LogInformation($"Item: {itemName}, InStock={dto.InStockDisplay}, Required={dto.RequiredDisplay}");
                }

                return materials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMaterialStatusAsync");
                throw;
            }
        }

        // ✅ Calculate material status dynamically for a single item (no DB writes)
        public async Task<MaterialStatusDto> CalculateMaterialStatusAsync(int projectId, string itemName)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            // ------------------------ YESTERDAY ------------------------
            var yesterdayStock = await _context.DailyStocks
                .FirstOrDefaultAsync(d =>
                    d.ProjectId == projectId &&
                    d.ItemName == itemName &&
                    d.Date == yesterday);

            decimal yesterdayInStock = yesterdayStock?.InStock ?? 0;       // physical
            decimal yesterdayRemaining = yesterdayStock?.RemainingQty ?? 0;



            // ------------------------ TODAY ------------------------
            decimal todayInward = await _context.StockInwards
                  .Where(x => x.ProjectId == projectId &&
                              x.Itemname == itemName &&
                              x.DateReceived.HasValue &&
                              x.DateReceived.Value.ToUniversalTime().Date == today)
                  .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

            decimal todayOutward = await _context.StockOutwards
                .Where(x => x.ProjectId == projectId &&
                            x.ItemName == itemName &&
                            x.DateIssued.HasValue &&
                            x.DateIssued.Value.ToUniversalTime().Date == today)
                .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);


            // ------------------------ CALCULATIONS ------------------------
            decimal todayInStock = yesterdayInStock + todayInward - todayOutward;
            if (todayInStock < 0) todayInStock = 0;

            decimal todayHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(itemName)
                ? DailyStockRequirement.RequiredStock[itemName]
                : 0;

            // Required = yesterday remaining + today hardcoded - today outward - today in-stock
            decimal requiredQty = yesterdayRemaining + todayHardcoded - todayOutward - todayInStock;
            if (requiredQty < 0) requiredQty = 0;

            // ------------------------ DETECT UNIT ------------------------
            string unit = await _context.StockInwards
                .Where(x => x.Itemname == itemName && !string.IsNullOrEmpty(x.Unit))
                .Select(x => x.Unit)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(unit))
            {
                unit = await _context.StockOutwards
                    .Where(x => x.ItemName == itemName && !string.IsNullOrEmpty(x.Unit))
                    .Select(x => x.Unit)
                    .FirstOrDefaultAsync();
            }

            if (string.IsNullOrEmpty(unit)) unit = "Units";

            // ------------------------ RETURN DTO ------------------------
            return new MaterialStatusDto
            {
                ItemName = itemName,
                InStockDisplay = $"{todayInStock:N2} {unit}",
                RequiredDisplay = $"{requiredQty:N2} {unit}"
            };
        }
        public async Task<List<MaterialStatusDto>> TriggerRecalculationIfNeededAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;

            bool inwardExists = await _context.StockInwards
                .AnyAsync(x => x.ProjectId == projectId &&
                               x.DateReceived.Value.ToUniversalTime().Date == today);

            bool outwardExists = await _context.StockOutwards
                .AnyAsync(x => x.ProjectId == projectId &&
                               x.DateIssued.Value.ToUniversalTime().Date == today);

            bool todayStockExists = await _context.DailyStocks
                .AnyAsync(x => x.ProjectId == projectId &&
                               x.Date == today);

            // 👇 THIS IS THE FIX: ensure DailyStock updates
            if (!todayStockExists)
            {
                await _dailyStockRepository.ResetDailyStockAsync(projectId);
            }
            else if (inwardExists || outwardExists)
            {
                await _dailyStockRepository.UpdateDailyStockForProjectAsync(projectId);
            }

            // Always return MaterialStatus
            return await GetMaterialStatusAsync(projectId);
        }


    }
}
