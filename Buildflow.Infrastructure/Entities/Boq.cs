using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Boq
{
    public int BoqId { get; set; }

    public string? BoqName { get; set; }

    public string? BoqCode { get; set; }

    public int? ProjectId { get; set; }

    public int? CreatedBy { get; set; }

    public int? VendorId { get; set; }

    public virtual ICollection<BoqApproval> BoqApprovals { get; set; } = new List<BoqApproval>();

    public virtual ICollection<BoqItem> BoqItems { get; set; } = new List<BoqItem>();

    public virtual EmployeeDetail? CreatedByNavigation { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual Vendor? Vendor { get; set; }
}
