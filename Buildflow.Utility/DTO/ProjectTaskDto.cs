using System;

namespace Buildflow.Utility.DTO
{
    public class ProjectTaskDto
    {
        public int TaskId { get; set; }
        public int MilestoneId { get; set; }
        public string? TaskCode { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? FinishedDate { get; set; }
        public int? DurationDays { get; set; }
        public int? DelayedDays { get; set; }
        public int? Status { get; set; }
        public string? Remarks { get; set; }
        
        public string? Unit { get; set; }
        public int? TotalScope { get; set; }
        public int? ExecutedWork { get; set; }
        public string? Location { get; set; }

        // ✅ Calculated only in DTO
        public int BalanceScope => (TotalScope ?? 0) - (ExecutedWork ?? 0);
    }
}
