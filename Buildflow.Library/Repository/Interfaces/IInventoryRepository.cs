using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IInventoryRepository
    {
        Task<StockInwardDto> CreateStockInwardAsync(StockInwardDto dto);
        Task<StockOutwardDto> CreateStockOutwardAsync(StockOutwardDto dto);

        Task<IEnumerable<StockInwardDto>> GetStockInwardsByProjectIdAsync(int projectId);
        Task<IEnumerable<StockOutwardDto>> GetStockOutwardsByProjectIdAsync(int projectId);


    }
}
