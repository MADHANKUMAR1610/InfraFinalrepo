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

        // ✅ Get Material Status (based on project)
        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                // ✅ Get all items linked to this project from inward & outward
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
                    // ✅ Yesterday inward/outward
                    decimal yesterdayInward = await _context.StockInwards
                        .Where(x => x.ProjectId == projectId &&
                                    x.Itemname == itemName &&
                                    x.DateReceived.HasValue &&
                                    x.DateReceived.Value.Date == yesterday)
                        .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                    decimal yesterdayOutward = await _context.StockOutwards
                        .Where(x => x.ProjectId == projectId &&
                                    x.ItemName == itemName &&
                                    x.DateIssued.HasValue &&
                                    x.DateIssued.Value.Date == yesterday)
                        .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                    // ✅ Yesterday in-stock = inward − outward
                    decimal yesterdayInStock = yesterdayInward - yesterdayOutward;
                    if (yesterdayInStock < 0)
                        yesterdayInStock = 0;

                    // ✅ Yesterday hardcoded requirement and balance
                    decimal yesterdayHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(itemName)
                        ? DailyStockRequirement.RequiredStock[itemName]
                        : 0;

                    decimal yesterdayHardcodedBalance = yesterdayHardcoded - yesterdayOutward;
                    if (yesterdayHardcodedBalance < 0)
                        yesterdayHardcodedBalance = 0;

                    // ✅ Today inward/outward
                    decimal todayInward = await _context.StockInwards
                        .Where(x => x.ProjectId == projectId &&
                                    x.Itemname == itemName &&
                                    x.DateReceived.HasValue &&
                                    x.DateReceived.Value.Date == today)
                        .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                    decimal todayOutward = await _context.StockOutwards
                        .Where(x => x.ProjectId == projectId &&
                                    x.ItemName == itemName &&
                                    x.DateIssued.HasValue &&
                                    x.DateIssued.Value.Date == today)
                        .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                    // ✅ TodayInStock = yesterdayInStock + todayInward − todayOutward
                    decimal todayInStock = yesterdayInStock + todayInward - todayOutward;
                    if (todayInStock < 0)
                        todayInStock = 0;

                    // ✅ Today hardcoded
                    decimal todayHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(itemName)
                        ? DailyStockRequirement.RequiredStock[itemName]
                        : 0;

                    // ✅ RequiredQty = (YesterdayHardcodedBalance + TodayHardcoded) − TodayInStock
                    decimal requiredQty = (yesterdayHardcodedBalance + todayHardcoded) - todayInStock;
                    if (requiredQty < 0)
                        requiredQty = 0;

                    // ✅ Get or create DailyStock entry (no ProjectId)
                    var todayStock = await _context.DailyStocks
                        .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date.Date == today);

                    if (todayStock == null)
                    {
                        todayStock = new DailyStock
                        {
                            ItemName = itemName,
                            DefaultQty = todayHardcoded,
                            RemainingQty = todayInStock,
                            Date = today
                        };
                        await _context.DailyStocks.AddAsync(todayStock);
                    }
                    else
                    {
                        todayStock.RemainingQty = todayInStock;
                        todayStock.DefaultQty = todayHardcoded;
                        _context.DailyStocks.Update(todayStock);
                    }

                    // ✅ Detect Unit
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

                    if (string.IsNullOrEmpty(unit))
                        unit = "Units";

                    // ✅ Add to material list
                    materials.Add(new MaterialStatusDto
                    {
                        ItemName = itemName,
                        InStockDisplay = $"{todayInStock:N2} {unit}",
                        RequiredDisplay = $"{requiredQty:N2} {unit}"
                    });

                    _logger.LogInformation($"Item: {itemName}, InStock={todayInStock}, Required={requiredQty}");
                }

                await _context.SaveChangesAsync();
                return materials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMaterialStatusAsync");
                throw;
            }
        }

        // ✅ UpdateDailyStockAsync (no ProjectId in DailyStock)
        public async Task UpdateDailyStockAsync(int projectId, string itemName)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                // ✅ Yesterday inward/outward
                decimal yesterdayInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId &&
                                x.Itemname == itemName &&
                                x.DateReceived.HasValue &&
                                x.DateReceived.Value.Date == yesterday)
                    .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                decimal yesterdayOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId &&
                                x.ItemName == itemName &&
                                x.DateIssued.HasValue &&
                                x.DateIssued.Value.Date == yesterday)
                    .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                // ✅ Yesterday in-stock = inward − outward
                decimal yesterdayInStock = yesterdayInward - yesterdayOutward;
                if (yesterdayInStock < 0)
                    yesterdayInStock = 0;

                // ✅ Yesterday hardcoded balance
                decimal yesterdayHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(itemName)
                    ? DailyStockRequirement.RequiredStock[itemName]
                    : 0;

                decimal yesterdayHardcodedBalance = yesterdayHardcoded - yesterdayOutward;
                if (yesterdayHardcodedBalance < 0)
                    yesterdayHardcodedBalance = 0;

                // ✅ Today inward/outward
                decimal todayInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId &&
                                x.Itemname == itemName &&
                                x.DateReceived.HasValue &&
                                x.DateReceived.Value.Date == today)
                    .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                decimal todayOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId &&
                                x.ItemName == itemName &&
                                x.DateIssued.HasValue &&
                                x.DateIssued.Value.Date == today)
                    .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                // ✅ Today in-stock
                decimal todayInStock = yesterdayInStock + todayInward - todayOutward;
                if (todayInStock < 0)
                    todayInStock = 0;

                // ✅ Today hardcoded & required
                decimal todayHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(itemName)
                    ? DailyStockRequirement.RequiredStock[itemName]
                    : 0;

                decimal requiredQty = (yesterdayHardcodedBalance + todayHardcoded) - todayInStock;
                if (requiredQty < 0)
                    requiredQty = 0;

                // ✅ Update or create DailyStock (no ProjectId)
                var todayStock = await _context.DailyStocks
                    .FirstOrDefaultAsync(d => d.ItemName == itemName && d.Date.Date == today);

                if (todayStock == null)
                {
                    todayStock = new DailyStock
                    {
                        ItemName = itemName,
                        DefaultQty = todayHardcoded,
                        RemainingQty = todayInStock,
                        Date = today
                    };
                    await _context.DailyStocks.AddAsync(todayStock);
                }
                else
                {
                    todayStock.RemainingQty = todayInStock;
                    todayStock.DefaultQty = todayHardcoded;
                    _context.DailyStocks.Update(todayStock);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Updated DailyStock for {itemName} | InStock={todayInStock} | Required={requiredQty}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateDailyStockAsync");
                throw;
            }
        }
    }
}
