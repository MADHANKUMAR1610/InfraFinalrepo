using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class BoqApproval
{
    public int BoqApprovalId { get; set; }

    public int? BoqId { get; set; }

    public int? TicketId { get; set; }

    public string? ApprovalStatus { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Boq? Boq { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
