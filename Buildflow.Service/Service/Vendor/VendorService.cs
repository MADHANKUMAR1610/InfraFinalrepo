using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Vendor
{
    public class VendorService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VendorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
    
        public async Task<BaseResponse> UpsertPurchaseOrderAsync(PurchaseOrderDto dto)
        {
            try
            {
                var purchaseOrder = await _unitOfWork.Vendors.UpsertPurchaseOrderAsync(dto);
                await _unitOfWork.CompleteAsync(); // this line is throwing
                return purchaseOrder;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("EF Core Save Error: " + dbEx.InnerException?.Message, dbEx);
            }
        }
        public async Task<List<VendorMaterialDetailsDto>> FetchVendorDetailsAsync(int vendorId)
        {
            return await _unitOfWork.Vendors.GetVendorAndMaterialDetailsAsync(vendorId);
        }
        public async Task<List<PurchaseOrderDetailsDto>> GetPurchaseOrderDetailsAsync(int purchase_order_id)
        {
            return await _unitOfWork.Vendors.GetPurchaseOrderDetailsAsync(purchase_order_id);
        } 
        
        public async Task<List<PurchaseOrderWithItemsDto>> GetApprovedPurchaseOrdersWithItemsAsync(int vendorId)
        {
            return await _unitOfWork.Vendors.GetApprovedPurchaseOrdersWithItemsAsync(vendorId);
        }
        public async Task<List<BoqPurchaseOrderDetailsDto>> GetPurchaseOrdersByBoqCodeAsync(string boqCode)
        {
            return await _unitOfWork.Vendors.GetPurchaseOrdersByBoqCodeAsync(boqCode);
        }

        public async Task<List<PurchaseOrderDetailsData>> GetAllPurchaseOrderDetailsAsync()
        {
            return await _unitOfWork.Vendors.GetAllPurchaseOrderDetailsAsync();
        }
    }
}



