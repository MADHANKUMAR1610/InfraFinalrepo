using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Library.UOW;
using Buildflow.Service.Service.Vendor;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Buildflow.Api.Controllers.Vendor
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorController : ControllerBase
    {
        private readonly VendorService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BuildflowAppContext _context;
        public VendorController(VendorService service, IConfiguration config, IUnitOfWork unitOfWork,BuildflowAppContext context)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpGet("Get-po-by-vendor-id")]
        public async Task<IActionResult> GetApprovedPurchaseOrdersWithItems(int vendorId)
        {
            var data = await _service.GetApprovedPurchaseOrdersWithItemsAsync(vendorId);
            return Ok(data);
        }

        [HttpGet("Getpurchase-orders-by-boq-code/{boqCode}")]
        public async Task<ActionResult<List<BoqPurchaseOrderDetailsDto>>> GetPurchaseOrdersByBoqCode(string boqCode)
        {
            var result = await _service.GetPurchaseOrdersByBoqCodeAsync(boqCode);
            if (result == null || result.Count == 0)
                return NotFound("No purchase orders found for the specified BOQ code.");
            return Ok(result);
        }

        [HttpPost("upsert-purchase-order")]
    
        public async Task<IActionResult> UpsertPurchaseOrder([FromBody] PurchaseOrderDto dto)
        {
            try
            {
                var result = await _service.UpsertPurchaseOrderAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("Get-Vendor/{vendorId}")]
        public async Task<IActionResult> GetVendor(int vendorId)
        {
            try
            {
                var result = await _service.FetchVendorDetailsAsync(vendorId);

                if (result == null || !result.Any())
                {
                    return NotFound("No vendor or material details found for the specified ID.");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        [HttpGet("Getpurchase-order-details/{purchase_order_id}")]
      
     
        public async Task<ActionResult<List<PurchaseOrderDetailsDto>>> GetPurchaseOrderDetails(int purchase_order_id)
        {
            var result = await _service.GetPurchaseOrderDetailsAsync(purchase_order_id);

            if (result == null || result.Count == 0)
                return NotFound("Purchase order details not found.");

            return Ok(result);
        }

        [HttpGet("Get-NewPOId")]
    

        public async Task<ActionResult> GetNewEmployeeId()
        {
            var po = await _context.PurchaseOrders.OrderByDescending(e => e.PurchaseOrderId).FirstOrDefaultAsync();
            Random random = new Random();
            int NewPOId = po != null ? po.PurchaseOrderId + 1 :0;

            return Ok("PO#" + NewPOId);
        }

        [HttpGet("Get-AllPurchaseOrderDetails")]  // Fixed the spelling mistake in the route
     
        public async Task<ActionResult<List<PurchaseOrderDetailsDto>>> GetAllPurchaseOrderDetails()
        {
            var result = await _service.GetAllPurchaseOrderDetailsAsync();

            if (result == null || result.Count == 0)
            {
                return NotFound("No purchase order details found.");
            }

            return Ok(result);
        }
    }
}

    






