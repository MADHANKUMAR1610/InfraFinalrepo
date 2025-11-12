using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Inventory
{
    public class DailyStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyStockBackgroundService> _logger;

  
        public DailyStockBackgroundService(IServiceProvider serviceProvider, ILogger<DailyStockBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dailyStockService = scope.ServiceProvider.GetRequiredService<DailyStockService>();
                        await dailyStockService.ResetDailyStockAsync();
                    }

                    Console.WriteLine($" Daily stock reset done at {DateTime.Now}");

                    // Wait until next midnight
                    var now = DateTime.Now;
                    var nextRun = DateTime.Today.AddDays(1).AddSeconds(5); // run at 12:00:05 AM
                    var delay = nextRun - now;

                    if (delay < TimeSpan.Zero)
                        delay = TimeSpan.FromHours(24); // safety fallback

                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" Error in DailyStockBackgroundService: {ex.Message}");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}
