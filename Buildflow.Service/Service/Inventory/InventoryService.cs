using Buildflow.Infrastructure.Entities;
using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Inventory
{
    public class InventoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<StockInwardDto> CreateStockInwardAsync(StockInwardDto dto)
        {
            return await _unitOfWork.InventoryRepository.CreateStockInwardAsync(dto);
        }
        public async Task<StockOutwardDto> CreateStockOutwardAsync(StockOutwardDto dto)
        {
            return await _unitOfWork.InventoryRepository.CreateStockOutwardAsync(dto);
        }
        public async Task<IEnumerable<StockInwardDto>> GetStockInwardsByProjectIdAsync(int projectId)
        {
            return await _unitOfWork.InventoryRepository.GetStockInwardsByProjectIdAsync(projectId);
        }

        public async Task<IEnumerable<StockOutwardDto>> GetStockOutwardsByProjectIdAsync(int projectId)
        {
            return await _unitOfWork.InventoryRepository.GetStockOutwardsByProjectIdAsync(projectId);
        }
        public async Task<IEnumerable<object>> GetProjectTeamMembersAsync(int projectId)
        {
            return await _unitOfWork.InventoryRepository.GetProjectTeamMembersAsync(projectId);
        }


    }
}
