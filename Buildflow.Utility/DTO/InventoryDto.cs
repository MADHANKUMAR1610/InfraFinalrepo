using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class StockInwardDto
    {
        public int? StockinwardId { get; set; }
        public int ProjectId { get; set; }
        public string? Grn { get; set; }
        public string? Itemname { get; set; }
        public int? VendorId { get; set; }
        public string? VendorName { get; set; } 
        public decimal? QuantityReceived { get; set; }
        public string? Unit { get; set; }
        public DateTime? DateReceived { get; set; }
        public int? ReceivedById { get; set; }
        public string? ReceivedByName { get; set; } 
        public string? Status { get; set; }
        public string? Remarks { get; set; }
    }

    public class StockOutwardDto
    {
        public int? StockOutwardId { get; set; }
        public int ProjectId { get; set; }
        public string? IssueNo { get; set; }
        public string? ItemName { get; set; }
        public int? RequestedById { get; set; }
        public string? RequestedByName { get; set; } 
        public decimal? IssuedQuantity { get; set; }
        public string? Unit { get; set; }
        public int? IssuedToId { get; set; }
        public string? IssuedToName { get; set; } 
        public DateTime? DateIssued { get; set; }
        public string? Status { get; set; }
        public string? Remarks { get; set; }
    }
}
