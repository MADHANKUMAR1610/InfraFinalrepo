using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Ticket
{
    public int TicketId { get; set; }

    public string? TicketNo { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int BoardId { get; set; }

    public int? LabelId { get; set; }

    public int? ApprovedBy { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? AssignBy { get; set; }

    public string? TicketLabel { get; set; }

    public string? TicketType { get; set; }

    public int? TransactionId { get; set; }

    public int? MoveBy { get; set; }

    public int? MoveTo { get; set; }

    public int? BoqId { get; set; }

    public int? Isapproved { get; set; }

    public virtual EmployeeDetail? ApprovedByNavigation { get; set; }

    public virtual EmployeeDetail? AssignByNavigation { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual Boq? Boq { get; set; }

    public virtual ICollection<BoqApproval> BoqApprovals { get; set; } = new List<BoqApproval>();

    public virtual EmployeeDetail? CreatedByNavigation { get; set; }

    public virtual EmployeeDetail? MoveByNavigation { get; set; }

    public virtual EmployeeDetail? MoveToNavigation { get; set; }

    public virtual ICollection<ProjectApproval> ProjectApprovals { get; set; } = new List<ProjectApproval>();

    public virtual ICollection<PurchaseOrderApproval> PurchaseOrderApprovals { get; set; } = new List<PurchaseOrderApproval>();

    public virtual ICollection<TicketAssignee> TicketAssignees { get; set; } = new List<TicketAssignee>();

    public virtual ICollection<TicketAttachment> TicketAttachments { get; set; } = new List<TicketAttachment>();

    public virtual ICollection<TicketComment> TicketComments { get; set; } = new List<TicketComment>();

    public virtual ICollection<TicketMovement> TicketMovements { get; set; } = new List<TicketMovement>();

    public virtual ICollection<TicketParticipant> TicketParticipants { get; set; } = new List<TicketParticipant>();

    public virtual EmployeeDetail? UpdatedByNavigation { get; set; }
}
