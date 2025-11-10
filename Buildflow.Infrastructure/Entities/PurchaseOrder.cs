using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class PurchaseOrder
{
    public int PurchaseOrderId { get; set; }

    public string? PoId { get; set; }

    public int BoqId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? Status { get; set; }

    public int? VendorId { get; set; }

    public string? DeliveryStatus { get; set; }

    public DateTime? DeliveryStatusDate { get; set; }

    public virtual Boq Boq { get; set; } = null!;

    public virtual EmployeeDetail? CreatedByNavigation { get; set; }

    public virtual ICollection<PurchaseOrderApproval> PurchaseOrderApprovals { get; set; } = new List<PurchaseOrderApproval>();

    public virtual ICollection<PurchaseOrderItem> PurchaseOrderItems { get; set; } = new List<PurchaseOrderItem>();
}
