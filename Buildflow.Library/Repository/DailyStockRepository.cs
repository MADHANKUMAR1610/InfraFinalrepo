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
        /// Resets or initializes daily stock per project
        /// </summary>
        public async Task ResetDailyStockAsync(int projectId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                bool alreadyExists = await _context.DailyStocks
                    .AnyAsync(d => d.ProjectId == projectId && d.Date.Date == today);

                if (alreadyExists)
                {
                    _logger.LogInformation($"Daily stock for project {projectId} already exists. Skipping reset.");
                    return;
                }

                var yesterdayStock = await _context.DailyStocks
                    .Where(d => d.ProjectId == projectId && d.Date.Date == yesterday)
                    .ToListAsync();

                var newStock = new List<DailyStock>();

                if (yesterdayStock.Any())
                {
                    foreach (var item in yesterdayStock)
                    {
                        decimal todayHardcoded = DailyStockRequirement.RequiredStock[item.ItemName];
                        decimal carryForward = item.RemainingQty; // yesterday’s balance requirement

                        newStock.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = item.ItemName,
                            DefaultQty = todayHardcoded, // today’s planned requirement
                            RemainingQty = carryForward + todayHardcoded, // add yesterday’s pending to today’s
                            Date = today
                        });
                    }

                    _logger.LogInformation($"Carried forward yesterday’s balance for project {projectId}.");
                }
                else
                {
                    foreach (var kvp in DailyStockRequirement.RequiredStock)
                    {
                        newStock.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = kvp.Key,
                            DefaultQty = kvp.Value,
                            RemainingQty = kvp.Value, // first day = full required amount
                            Date = today
                        });
                    }

                    _logger.LogInformation($"Initialized first-day stock for project {projectId}.");
                }

                await _context.DailyStocks.AddRangeAsync(newStock);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting daily stock for project {ProjectId}", projectId);
                throw;
            }
        }

    }
}