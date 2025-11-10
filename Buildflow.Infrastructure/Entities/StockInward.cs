using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class StockInward
{
    public int StockinwardId { get; set; }

    public int ProjectId { get; set; }

    public string? Grn { get; set; }

    public string? Itemname { get; set; }

    public int? VendorId { get; set; }

    public decimal? QuantityReceived { get; set; }

    public string? Unit { get; set; }

    public DateTime? DateReceived { get; set; }

    public int? ReceivedbyId { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual EmployeeDetail? Receivedby { get; set; }

    public virtual Vendor? Vendor { get; set; }
}
