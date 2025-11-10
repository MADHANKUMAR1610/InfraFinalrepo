using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class TicketParticipant
{
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int ParticipantId { get; set; }

    public int AddedBy { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual EmployeeDetail AddedByNavigation { get; set; } = null!;

    public virtual EmployeeDetail Participant { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
