using Buildflow.Library.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Inventory
{
    public class DailyStockService
    {
        private readonly IDailyStockRepository _dailyStockRepository;

        public DailyStockService(IDailyStockRepository dailyStockRepository)
        {
            _dailyStockRepository = dailyStockRepository;
        }

        public async Task ResetDailyStockAsync()
        {
            await _dailyStockRepository.ResetDailyStockAsync();
        }
    }
}
