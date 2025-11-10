using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectRiskManagement
{
    public int RiskId { get; set; }

    public int ProjectId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? RiskDescription { get; set; }

    public string RiskStatus { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public string? RiskImpact { get; set; }

    public decimal? RiskProbability { get; set; }

    public string? MitigationPlan { get; set; }

    public DateOnly? IdentifiedDate { get; set; }

    public DateOnly? ResolvedDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Remarks { get; set; }

    public virtual Project Project { get; set; } = null!;
}
