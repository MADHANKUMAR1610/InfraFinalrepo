using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectMilestone
{
    public int MilestoneId { get; set; }

    public int ProjectId { get; set; }

    public string MilestoneName { get; set; } = null!;

    public string? MilestoneDescription { get; set; }

    public DateOnly? MilestoneStartDate { get; set; }

    public DateOnly? MilestoneEndDate { get; set; }

    public string? MilestoneStatus { get; set; }

    public decimal? MilestoneBudget { get; set; }

    public decimal? ActualCost { get; set; }

    public decimal? CompletionPercentage { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Remarks { get; set; }

    public virtual Project Project { get; set; } = null!;
}
