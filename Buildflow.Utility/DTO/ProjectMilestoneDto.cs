using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProjectMilestoneDto
{
    public int MilestoneId { get; set; }
    public int ProjectId { get; set; }
    public string MilestoneName { get; set; }
    public string? MilestoneDescription { get; set; }
    public DateOnly? MilestoneStartDate { get; set; }
    public DateOnly? MilestoneEndDate { get; set; }
    public string? MilestoneStatus { get; set; }
    public string? Remarks { get; set; }
}