using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class TicketAssignee
{
    public int TicketAssigneeId { get; set; }

    public int? TicketId { get; set; }

    public int? AssigneeId { get; set; }

    public int? AssignBy { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
