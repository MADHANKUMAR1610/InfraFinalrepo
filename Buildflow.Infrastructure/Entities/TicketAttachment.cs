using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class TicketAttachment
{
    public int AttachmentId { get; set; }

    public int TicketId { get; set; }

    public string? FileName { get; set; }

    public string? FilePath { get; set; }

    public int CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? TicketCommentId { get; set; }

    public virtual Ticket Ticket { get; set; } = null!;

    public virtual TicketComment? TicketComment { get; set; }
}
