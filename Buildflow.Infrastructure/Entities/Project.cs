using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Project
{
    public int ProjectId { get; set; }

    public string? ProjectCode { get; set; }

    public string ProjectName { get; set; } = null!;

    public string? ProjectDescription { get; set; }

    public int ProjectTypeId { get; set; }

    public int? ProjectSectorId { get; set; }

    public string ProjectStatus { get; set; } = null!;

    public DateOnly ProjectStartDate { get; set; }

    public DateOnly? ProjectEndDate { get; set; }

    public decimal ProjectTotalBudget { get; set; }

    public decimal? ProjectActualCost { get; set; }

    public string? ClientName { get; set; }

    public string? ProjectPriority { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? ProjectLocation { get; set; }

    public virtual ICollection<Boq> Boqs { get; set; } = new List<Boq>();

    public virtual ICollection<ProjectApproval> ProjectApprovals { get; set; } = new List<ProjectApproval>();

    public virtual ICollection<ProjectBudgetDetail> ProjectBudgetDetails { get; set; } = new List<ProjectBudgetDetail>();

    public virtual ICollection<ProjectDetail> ProjectDetails { get; set; } = new List<ProjectDetail>();

    public virtual ICollection<ProjectMilestone> ProjectMilestones { get; set; } = new List<ProjectMilestone>();

    public virtual ICollection<ProjectPermissionFinanceApproval> ProjectPermissionFinanceApprovals { get; set; } = new List<ProjectPermissionFinanceApproval>();

    public virtual ICollection<ProjectRiskManagement> ProjectRiskManagements { get; set; } = new List<ProjectRiskManagement>();

    public virtual ProjectSector? ProjectSector { get; set; }

    public virtual ICollection<ProjectTeam> ProjectTeams { get; set; } = new List<ProjectTeam>();

    public virtual ProjectType ProjectType { get; set; } = null!;
}
