using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectTask
{
    public int TaskId { get; set; }

    public int MilestoneId { get; set; }

    public string? TaskCode { get; set; }

    public string TaskName { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? PlannedEndDate { get; set; }

    public DateOnly? FinishedDate { get; set; }

    public int? DurationDays { get; set; }

    public int? DelayedDays { get; set; }

    public int? Status { get; set; }

    public string? Remarks { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ProjectMilestone Milestone { get; set; } = null!;
}
