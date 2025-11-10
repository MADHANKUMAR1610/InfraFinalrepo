using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectTeam
{
    public int ProjectTeamId { get; set; }

    public int ProjectId { get; set; }

    public List<int>? PmId { get; set; }

    public List<int>? ApmId { get; set; }

    public List<int>? LeadEnggId { get; set; }

    public List<int>? SiteSupervisorId { get; set; }

    public List<int>? QsId { get; set; }

    public List<int>? AqsId { get; set; }

    public List<int>? SiteEnggId { get; set; }

    public List<int>? EnggId { get; set; }

    public List<int>? DesignerId { get; set; }

    public List<int>? VendorId { get; set; }

    public List<int>? SubcontractorId { get; set; }

    public DateOnly? AssignmentStartDate { get; set; }

    public DateOnly? AssignmentEndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Project Project { get; set; } = null!;
}
