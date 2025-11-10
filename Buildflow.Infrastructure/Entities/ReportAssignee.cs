using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ReportAssignee
{
    public int ReportAssigneeId { get; set; }

    public int ReportId { get; set; }

    public int AssigneeId { get; set; }

    public int? SendBy { get; set; }

    public virtual EmployeeDetail Assignee { get; set; } = null!;

    public virtual Report Report { get; set; } = null!;

    public virtual EmployeeDetail? SendByNavigation { get; set; }
}
