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
using System.Text;
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


        public async Task ResetDailyStockAsync()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var yesterday = today.AddDays(-1);

                bool alreadyExists = await _context.DailyStocks.AnyAsync(d => d.Date.Date == today);
                if (alreadyExists)
                {
                    _logger.LogInformation("Daily stock for today already exists. Skipping reset.");
                    return;
                }

                var yesterdayStock = await _context.DailyStocks
                    .Where(d => d.Date.Date == yesterday)
                    .ToListAsync();

                var newStock = new List<DailyStock>();

                if (yesterdayStock.Any())
                {
                    // Carry forward balance from yesterday
                    foreach (var item in yesterdayStock)
                    {
                        newStock.Add(new DailyStock
                        {
                            ItemName = item.ItemName,
                            DefaultQty = item.DefaultQty,
                            RemainingQty = item.RemainingQty + item.DefaultQty,
                            Date = DateTime.UtcNow
                        });
                    }

                    _logger.LogInformation("Daily stock reset successful (carried forward yesterday’s balance).");
                }
                else
                {
                    // First day: insert default hardcoded stock
                    foreach (var kvp in DailyStockRequirement.RequiredStock)
                    {
                        newStock.Add(new DailyStock
                        {
                            ItemName = kvp.Key,
                            DefaultQty = kvp.Value,
                            RemainingQty = kvp.Value,
                            Date = DateTime.UtcNow
                        });
                    }

                    _logger.LogInformation("Initial daily stock inserted using hardcoded defaults.");
                }

                await _context.DailyStocks.AddRangeAsync(newStock);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while resetting daily stock: {Message}", ex.Message);
                throw new ApplicationException("Error while resetting daily stock", ex);
            }
        }
    }
}

    