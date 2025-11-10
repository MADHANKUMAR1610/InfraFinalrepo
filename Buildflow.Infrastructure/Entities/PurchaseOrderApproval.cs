using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class PurchaseOrderApproval
{
    public int PurchaseOrderApprovalId { get; set; }

    public int? PurchaseOrderId { get; set; }

    public int? TicketId { get; set; }

    public string? ApprovalStatus { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual PurchaseOrder? PurchaseOrder { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
