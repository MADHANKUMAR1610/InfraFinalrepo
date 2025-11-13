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
        private readonly IConfiguration _configuration;

        public DailyStockRepository(
            IConfiguration configuration,
            BuildflowAppContext context,
            ILogger<DailyStockRepository> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Reset or initialize daily stock at start of the day
        /// </summary>
        public async Task ResetDailyStockAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                bool todayExists = await _context.DailyStocks
                    .AnyAsync(d => d.ProjectId == projectId && d.Date.Date == today);

                if (todayExists)
                {
                    _logger.LogInformation($"Daily stock for project {projectId} already exists. Skipping reset.");
                    return;
                }

                var yesterdayStocks = await _context.DailyStocks
                    .Where(d => d.ProjectId == projectId && d.Date.Date == yesterday)
                    .ToListAsync();

                var newStocks = new List<DailyStock>();

                if (yesterdayStocks.Any())
                {
                    // Day 2+: carry forward yesterday's remaining
                    foreach (var item in yesterdayStocks)
                    {
                        decimal todayHardcoded = DailyStockRequirement.RequiredStock[item.ItemName];
                        decimal remainingQty = todayHardcoded + item.RemainingQty;

                        newStocks.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = item.ItemName,
                            DefaultQty = todayHardcoded,
                            RemainingQty = remainingQty,
                            Date = today
                        });
                    }

                    _logger.LogInformation($"Carried forward yesterday's remaining stock for project {projectId}.");
                }
                else
                {
                    // Day 1: initialize from hardcoded values
                    foreach (var kvp in DailyStockRequirement.RequiredStock)
                    {
                        newStocks.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = kvp.Key,
                            DefaultQty = kvp.Value,
                            RemainingQty = kvp.Value,
                            Date = today
                        });
                    }

                    _logger.LogInformation($"Initialized first-day stock for project {projectId}.");
                }

                await _context.DailyStocks.AddRangeAsync(newStocks);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting daily stock for project {ProjectId}", projectId);
                throw;
            }
        }

        /// <summary>
        /// Update remaining quantity whenever new outward is created
        /// Only RemainingQty is updated
        /// </summary>
        public async Task UpdateDailyStockAsync(int projectId, string itemName, decimal outwardQty)
        {
            try
            {
                var today = DateTime.UtcNow.Date;

                var todayStock = await _context.DailyStocks
                    .FirstOrDefaultAsync(d => d.ProjectId == projectId && d.ItemName == itemName && d.Date.Date == today);

                if (todayStock == null)
                {
                    _logger.LogWarning($"Daily stock not found for {itemName} on project {projectId}. Resetting daily stock first.");
                    await ResetDailyStockAsync(projectId);
                    todayStock = await _context.DailyStocks
                        .FirstOrDefaultAsync(d => d.ProjectId == projectId && d.ItemName == itemName && d.Date.Date == today);
                }

                if (todayStock != null)
                {
                    todayStock.RemainingQty -= outwardQty;
                    if (todayStock.RemainingQty < 0)
                        todayStock.RemainingQty = 0;

                    _context.DailyStocks.Update(todayStock);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Updated RemainingQty for {itemName} on project {projectId}. Outward={outwardQty}, RemainingQty={todayStock.RemainingQty}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating daily stock for project {ProjectId}, item {ItemName}", projectId, itemName);
                throw;
            }
        }

        /// <summary>
        /// Get current daily stock for a project
        /// Only RemainingQty is returned
        /// </summary>
        public async Task<List<(string ItemName, decimal RemainingQty)>> GetDailyStockAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var result = new List<(string, decimal)>();

            var todayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date.Date == today)
                .ToListAsync();

            foreach (var item in todayStocks)
            {
                result.Add((item.ItemName, item.RemainingQty));
            }

            return result;
        }
    }
}
