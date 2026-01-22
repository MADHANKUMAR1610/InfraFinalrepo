using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class ProjectSubTaskDto
    {
        public int SubtaskId { get; set; }

        public int TaskId { get; set; }

        public string? SubtaskCode { get; set; }

        public string SubtaskName { get; set; } = null!;

        public DateTime? StartDate { get; set; }

        public DateTime? PlannedEndDate { get; set; }

        public DateTime? FinishedDate { get; set; }

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

        public int BalanceScope => (TotalScope ?? 0) - (ExecutedWork ?? 0);
    }
}
