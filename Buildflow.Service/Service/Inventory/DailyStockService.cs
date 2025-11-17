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

        //reset daily stock at start of the day
        public async Task ResetDailyStockAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("Invalid project ID provided.", nameof(projectId));

            await _dailyStockRepository.ResetDailyStockAsync(projectId);
        }
    }
}
