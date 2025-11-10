using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectApproval
{
    public int ApprovalId { get; set; }

    public int? ProjectId { get; set; }

    public string? ApprovalType { get; set; }

    public string? Status { get; set; }

    public int? ApprovedBy { get; set; }

    public DateOnly? ApprovalDate { get; set; }

    public string? Comments { get; set; }

    public string? DocumentUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? TicketId { get; set; }

    public virtual EmployeeDetail? ApprovedByNavigation { get; set; }

    public virtual Project? Project { get; set; }

    public virtual Ticket? Ticket { get; set; }
}
