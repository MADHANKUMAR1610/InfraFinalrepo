using Buildflow.Infrastructure.Entities;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IDailyStockRepository
    {
        Task ResetDailyStockAsync(int projectId);

        Task UpdateDailyStockAsync(
            int projectId,
            string itemName,
            decimal outwardQty = 0,
            decimal inwardQty = 0);

        Task UpdateDailyStockForProjectAsync(int projectId);

        Task<List<(string ItemName, decimal RemainingQty)>> GetDailyStockAsync(int projectId);

        Task AddNewBoqItemsToDailyStockAsync(int projectId, int boqId);

        // ➕ ADD THIS
        Task SaveCalculatedStockAsync(
       int projectId,
       string itemName,
       decimal instock,
       decimal requiredQty);

    }
}
