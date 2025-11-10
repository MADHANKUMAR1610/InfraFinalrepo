using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class StockOutward
{
    public int StockOutwardId { get; set; }

    public int ProjectId { get; set; }

    public string? IssueNo { get; set; }

    public string? ItemName { get; set; }

    public int? RequestedById { get; set; }

    public int? IssuedToId { get; set; }

    public string? Unit { get; set; }

    public decimal? IssuedQuantity { get; set; }

    public DateTime? DateIssued { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public virtual EmployeeDetail? IssuedTo { get; set; }

    public virtual Project Project { get; set; } = null!;

    public virtual EmployeeDetail? RequestedBy { get; set; }
}
