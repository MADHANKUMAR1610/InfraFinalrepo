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

        // ✅ Get material status for display (calculates dynamically, no DB writes)
        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                // Get all items linked to this project from inward & outward
                var allItems = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId)
                    .Select(x => x.Itemname)
                    .Union(
                        _context.StockOutwards
                        .Where(x => x.ProjectId == projectId)
                        .Select(x => x.ItemName)
                    )
                    .Distinct()
                    .ToListAsync();

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
            decimal yesterdayInward = await _context.StockInwards
                .Where(x => x.ProjectId == projectId &&
                            x.Itemname == itemName &&
                            x.DateReceived.HasValue &&
                            x.DateReceived.Value.ToUniversalTime().Date == yesterday)
                .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

            decimal yesterdayOutward = await _context.StockOutwards
                .Where(x => x.ProjectId == projectId &&
                            x.ItemName == itemName &&
                            x.DateIssued.HasValue &&
                            x.DateIssued.Value.ToUniversalTime().Date == yesterday)
                .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

            decimal yesterdayInStock = yesterdayInward - yesterdayOutward;
            if (yesterdayInStock < 0) yesterdayInStock = 0;

            var yesterdayStock = await _context.DailyStocks
                .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date.Date == yesterday);

            decimal yesterdayRemaining = yesterdayStock?.RemainingQty ?? 0; // Already calculated: yesterday hardcoded - yesterday outward

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

        // ✅ Trigger recalculation only if new inward or outward exists (or first call of day)
        public async Task<List<MaterialStatusDto>> TriggerRecalculationIfNeededAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;

            bool inwardExists = await _context.StockInwards
                .AnyAsync(x => x.ProjectId == projectId && x.DateReceived.Value.ToUniversalTime().Date == today);

            bool outwardExists = await _context.StockOutwards
                .AnyAsync(x => x.ProjectId == projectId && x.DateIssued.Value.ToUniversalTime().Date == today);

            if (!inwardExists && !outwardExists)
            {
                // Nothing new today, just return calculated values from yesterday remaining
                var items = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId)
                    .Select(x => x.Itemname)
                    .Union(
                        _context.StockOutwards
                        .Where(x => x.ProjectId == projectId)
                        .Select(x => x.ItemName)
                    )
                    .Distinct()
                    .ToListAsync();

                var materials = new List<MaterialStatusDto>();
                foreach (var item in items)
                {
                    var dto = await CalculateMaterialStatusAsync(projectId, item);
                    materials.Add(dto);
                }
                return materials;
            }
            else
            {
                // New movement today, recalculate
                return await GetMaterialStatusAsync(projectId);
            }
        }
    }
}
