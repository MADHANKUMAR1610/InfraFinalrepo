using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class EmployeeDetail
{
    public int EmpId { get; set; }

    public string EmployeeCode { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public string? Password { get; set; }

    public string? Phone { get; set; }

    public string? AlternativePhone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public DateOnly? HireDate { get; set; }

    public DateOnly? TerminationDate { get; set; }

    public bool? IsActive { get; set; }

    public string? Address { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? IsAllocated { get; set; }

    public string? Gender { get; set; }

    public virtual ICollection<Board> BoardCreatedByNavigations { get; set; } = new List<Board>();

    public virtual ICollection<Board> BoardUpdatedByNavigations { get; set; } = new List<Board>();

    public virtual ICollection<Boardlabel> BoardlabelCreatedByNavigations { get; set; } = new List<Boardlabel>();

    public virtual ICollection<Boardlabel> BoardlabelUpdatedByNavigations { get; set; } = new List<Boardlabel>();

    public virtual ICollection<Boq> Boqs { get; set; } = new List<Boq>();

    public virtual ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();

    public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<ProjectApproval> ProjectApprovals { get; set; } = new List<ProjectApproval>();

    public virtual ICollection<ProjectPermissionFinanceApproval> ProjectPermissionFinanceApprovals { get; set; } = new List<ProjectPermissionFinanceApproval>();

    public virtual ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();

    public virtual ICollection<ReportAssignee> ReportAssigneeAssignees { get; set; } = new List<ReportAssignee>();

    public virtual ICollection<ReportAssignee> ReportAssigneeSendByNavigations { get; set; } = new List<ReportAssignee>();

    public virtual ICollection<Ticket> TicketApprovedByNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketAssignByNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketCreatedByNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketMoveByNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<Ticket> TicketMoveToNavigations { get; set; } = new List<Ticket>();

    public virtual ICollection<TicketParticipant> TicketParticipantAddedByNavigations { get; set; } = new List<TicketParticipant>();

    public virtual ICollection<TicketParticipant> TicketParticipantParticipants { get; set; } = new List<TicketParticipant>();

    public virtual ICollection<Ticket> TicketUpdatedByNavigations { get; set; } = new List<Ticket>();
}
