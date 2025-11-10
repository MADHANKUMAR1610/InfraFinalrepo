using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class VendorDto
    {
    }
   

    public class BoqPurchaseOrderDetailsDto
    {
        [Column("boq_id")]
        public int BoqId { get; set; }

        [Column("boq_code")]
        public string BoqCode { get; set; }

        [Column("boq_name")]
        public string BoqName { get; set; }

        [Column("purchase_order_id")]
        public int? PurchaseOrderId { get; set; }

        [Column("po_id")]
        public string? PoId { get; set; }

        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Column("vendor_name")]
        public string VendorName { get; set; }

        [Column("vendor_email")]
        public string VendorEmail { get; set; }

        [Column("vendor_mobile")]
        public string VendorMobile { get; set; }

        public List<BoqPurchaseOrderItemDto> PurchaseOrderItems { get; set; }
    }

    public class BoqPurchaseOrderItemDto
    {
        [Column("purchase_order_items_id")]
        public int PurchaseOrderItemsId { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("total")]
        public double Total { get; set; }
    }

    public class PurchaseOrderItemDto
    {
        public int? PurchaseOrderItemsId { get; set; }
        public string? ItemName { get; set; }
        public string? Unit { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public double? Total { get; set; }
    }

    public class PurchaseOrderWithItemsDto
    {
        public int PurchaseOrderId { get; set; }
        public string PoId { get; set; }
        public int BoqId { get; set; }
        public int CreatedBy { get; set; }
        public string PurchaseManagerMobile { get; set; }
        

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public string? DeliveryStatus { get; set; }
        public DateTime? DeliveryStatusDate { get; set; }
        public DateTime? POReceivedDate { get; set; }
        public int VendorId { get; set; }
        public string VendorMobile { get; set; }
        public string? ProjectName { get; set; }
        public List<PurchaseOrderItemDto> Items { get; set; } = new();
    }


    public class VendorMaterialDetailsDto
    {
        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Column("vendorname")]
        public string VendorName { get; set; }

        [Column("contactnumber")]
        public string ContactNumber { get; set; }

        [Column("email")]
        public string Email { get; set; }

        public List<MaterialDetailDto> Materials { get; set; }
    }

    public class MaterialDetailDto
    {
        [Column("materialid")]
        public int MaterialId { get; set; }

        [Column("materialname")]
        public string MaterialName { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

        [Column("currentprice")]
        public decimal CurrentPrice { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("materialcategoryid")]
        public int MaterialCategoryId { get; set; }

        [Column("materialcategoryname")]
        public string MaterialCategoryName { get; set; }
    }

    public class PurchaseOrderDetailsData
    {
        [Column("purchase_order_id")]
        public int PurchaseOrderId { get; set; }

        [Column("po_id")]
        public string PoId { get; set; }

        [Column("boq_id")]
        public int BoqId { get; set; }

        [Column("boq_name")]
        public string BoqName { get; set; }

        [Column("project_id")]
        public int ProjectId { get; set; }

        [Column("project_name")]
        public string ProjectName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("delivery_status")]
        public string DeliveryStatus { get; set; }

        [Column("delivery_status_date")]
        public DateTime? DeliveryStatusDate { get; set; }

        [Column("vendor")]
        public string Vendor { get; set; }

        [Column("vendor_mobile_number")]
        public string VendorMobileNumber { get; set; }

        public List<PurchaseOrderItemDetailDto> PurchaseOrderItems { get; set; }

        [Column("approvers")]
        public List<ApproverDto> Approvers { get; set; }
    }

    public class PurchaseOrderDetailsDto
    {
        [Column("purchase_order_id")]
        public int PurchaseOrderId { get; set; }

        [Column("po_id")]
        public string PoId { get; set; }

        [Column("boq_id")]
        public int BoqId { get; set; }

        [Column("boq_code")]
        public string BoqCode { get; set; }

        [Column("boq_name")]
        public string BoqName { get; set; }

        [Column("vendor_id")]
        public int VendorId { get; set; }

        [Column("vendor_name")]
        public string VendorName { get; set; }

        [Column("project_id")]
        public int ProjectId { get; set; }

        [Column("project_name")]
        public string ProjectName { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("delivery_status")]
        public string? DeliveryStatus { get; set; }

        [Column("delivery_status_date")]
        public DateTime? DeliveryStatusDate { get; set; }


        public List<PurchaseOrderItemDetailDto> PurchaseOrderItems { get; set; }

        [Column("approvers")]
        public List<ApproverDto> Approvers { get; set; }
    }

   

    public class ApproverDto
    {
        [JsonPropertyName("employeeName")]
        public string EmployeeName { get; set; }

        [JsonPropertyName("roleName")]
        public string RoleName { get; set; }
    }

    public class PurchaseOrderItemDetailDto
    {
        [Column("purchase_order_items_id")]
        public int PurchaseOrderItemsId { get; set; }

        [Column("item_name")]
        public string ItemName { get; set; }

        [Column("unit")]
        public string Unit { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("quantity")]
        public int Quantity { get; set; }

        [Column("total")]
        public double Total { get; set; }
    }
    //public class VendorMaterialDetailsDto
    //{
    //    public int VendorId { get; set; }
    //    public string VendorName { get; set; }
    //    public string ContactNumber { get; set; }
    //    public string Email { get; set; }
    //    public int? MaterialId { get; set; }
    //    public string MaterialName { get; set; }
    //    public string Unit { get; set; }
    //    public decimal? CurrentPrice { get; set; }
    //    public decimal? Price { get; set; }
    //    public int? MaterialCategoryId { get; set; }
    //    public string MaterialCategoryName { get; set; }
    //}
    public class LoginVendorFullDetailDto
    {
      
            public int VendorId { get; set; }
            public string VendorCode { get; set; }
            public string VendorName { get; set; }
            public string OrganizationName { get; set; }
            public string Mobile { get; set; }
            public string AlternativeMobile { get; set; }
            public string Email { get; set; }
            public string Website { get; set; }
            public string Street { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string Country { get; set; }
            public string PostalCode { get; set; }
            public string TaxId { get; set; }
            public string PaymentTerms { get; set; }
            public string PreferredPaymentMethod { get; set; }
            public string DeliveryTerms { get; set; }
            public decimal? CreditLimit { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime UpdatedAt { get; set; }
            public int CreatedBy { get; set; }
            public int UpdatedBy { get; set; }
            public string Password { get; set; }
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string RoleCode { get; set; }
        

    }
    public class VendorProject
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class VendorNotification
    {
        public int NotificationId { get; set; }
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class PurchaseOrderDto
    {
        public int? PurchaseOrderId { get; set; }
        public string? PoId { get; set; }
        public string? BoqCode { get; set; }
        public int? CreatedBy { get; set; }
        public string? status { get; set; }
        public string? DeliveryStatus { get; set; }
        public DateTime? DeliveryStatusDate { get; set; }
        public List<PurchaseItemDto>? Items { get; set; }
        public List<int>? AssignTo { get; set; } = new List<int>();
        public string? TicketType { get; set; }
    }

    public class PurchaseItemDto
    {
        public int? ItemId { get; set; }
        public string? ItemName { get; set; }
        public string? Unit { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? Total { get; set; }
    }
}
