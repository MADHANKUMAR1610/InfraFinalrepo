using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Buildflow.Service.Service.Inventory;
using Buildflow.Infrastructure.DatabaseContext;

namespace Buildflow.Service.Service.Inventory
{
    public class DailyStockBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DailyStockBackgroundService> _logger;

        public DailyStockBackgroundService(IServiceProvider serviceProvider, ILogger<DailyStockBackgroundService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<BuildflowAppContext>();
                        var dailyStockService = scope.ServiceProvider.GetRequiredService<DailyStockService>();

                        // Fetch all active projects
                        var activeProjects = await Task.Run(() =>
                            context.Projects
                                .Where(p => p.IsActive == true)

                                .Select(p => p.ProjectId)
                                .ToList()
                        );

                        if (!activeProjects.Any())
                        {
                            _logger.LogWarning("No active projects found for daily stock reset.");
                        }
                        else
                        {
                            foreach (var projectId in activeProjects)
                            {
                                try
                                {
                                    await dailyStockService.ResetDailyStockAsync(projectId);
                                    _logger.LogInformation("✅ Daily stock reset done for Project ID: {ProjectId} at {Time}", projectId, DateTime.Now);
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError(ex, "❌ Error resetting stock for Project ID: {ProjectId}", projectId);
                                }
                            }
                        }
                    }

                    // Wait until next midnight (12:00:05 AM)
                    var now = DateTime.Now;
                    var nextRun = DateTime.Today.AddDays(1).AddSeconds(5);
                    var delay = nextRun - now;

                    if (delay < TimeSpan.Zero)
                        delay = TimeSpan.FromHours(24); // fallback

                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in DailyStockBackgroundService");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }
    }
}
