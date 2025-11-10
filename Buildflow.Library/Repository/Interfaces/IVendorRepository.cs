using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IVendorRepository
    {
        Task<List<VendorMaterialDetailsDto>> GetVendorsAsync(int vendorId);
        Task<BaseResponse> UpsertPurchaseOrderAsync(PurchaseOrderDto dto);
        Task<List<VendorMaterialDetailsDto>> GetVendorAndMaterialDetailsAsync(int vendorId);
        Task<List<PurchaseOrderDetailsDto>> GetPurchaseOrderDetailsAsync(int purchase_order_id);
        Task<List<BoqPurchaseOrderDetailsDto>> GetPurchaseOrdersByBoqCodeAsync(string boqCode);
        Task <List<PurchaseOrderWithItemsDto>> GetApprovedPurchaseOrdersWithItemsAsync(int vendorId);
        Task<List<PurchaseOrderDetailsData>> GetAllPurchaseOrderDetailsAsync();
    }
}

