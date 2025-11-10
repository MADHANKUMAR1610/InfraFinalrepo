using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class TicketMovement
{
    public int MovementId { get; set; }

    public int? TicketId { get; set; }

    public int? MoveBy { get; set; }

    public int? MoveTo { get; set; }

    public DateTime? MovedAt { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
