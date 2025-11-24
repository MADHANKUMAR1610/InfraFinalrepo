using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ProjectMilestoneInputDto
{
    public int ProjectId { get; set; }
    public List<ProjectMilestoneDto> MilestoneList { get; set; }
}

public class ProjectMilestoneDto
{
    public int MilestoneId { get; set; }
    public string MilestoneName { get; set; }
    public string? MilestoneDescription { get; set; }
    public DateTime? MilestoneStartDate { get; set; } // changed from DateOnly
    public DateTime? MilestoneEndDate { get; set; }   // changed from DateOnly
    public int Status { get; set; }               // name fixed
    public string? Remarks { get; set; }
}
