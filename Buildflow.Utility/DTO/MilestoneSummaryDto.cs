using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class MilestoneSummaryDto
    {
        public int MilestoneId { get; set; }
        public string MilestoneName { get; set; }

        public string? Location { get; set; }
        public string? Remarks { get; set; }

        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public DateOnly? FinishedDate { get; set; }

        public int TotalScope { get; set; }
        public int ExecutedWork { get; set; }
        public int BalanceScope { get; set; }

        public string CompletedStatus { get; set; }

        public int DurationDays { get; set; }
        public int DelayDays { get; set; }
    }
}
