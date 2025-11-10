using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Report
{
    public int ReportId { get; set; }

    public string? ReportCode { get; set; }

    public int ReportType { get; set; }

    public int? ProjectId { get; set; }

    public DateTime ReportDate { get; set; }

    public string ReportedBy { get; set; } = null!;

    public string ReportData { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ReportAssignee> ReportAssignees { get; set; } = new List<ReportAssignee>();

    public virtual ICollection<ReportAttachment> ReportAttachments { get; set; } = new List<ReportAttachment>();
}
