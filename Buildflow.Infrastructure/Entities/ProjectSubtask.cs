using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectSubtask
{
    public int SubtaskId { get; set; }

    public int TaskId { get; set; }

    public string? SubtaskCode { get; set; }

    public string SubtaskName { get; set; } = null!;

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

    public string? Unit { get; set; }

    public int? TotalScope { get; set; }

    public int? ExecutedWork { get; set; }

    public string? Location { get; set; }

    public virtual ProjectTask Task { get; set; } = null!;
}
