using Buildflow.Library.Repository.Interfaces;
using System;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Inventory
{
    public class DailyStockService
    {
        private readonly IDailyStockRepository _dailyStockRepository;

        public DailyStockService(IDailyStockRepository dailyStockRepository)
        {
            _dailyStockRepository = dailyStockRepository ?? throw new ArgumentNullException(nameof(dailyStockRepository));
        }

        /// <summary>
        /// Resets the daily stock for a specific project.
        /// Carries forward yesterday's remaining quantity and adds today's planned (hardcoded) requirement.
        /// </summary>
        /// <param name="projectId">The ID of the project to reset daily stock for.</param>
        public async Task ResetDailyStockAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID provided.", nameof(projectId));

            await _dailyStockRepository.ResetDailyStockAsync(projectId);
        }
    }
}
