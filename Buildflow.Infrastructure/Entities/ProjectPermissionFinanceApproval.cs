using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class ProjectPermissionFinanceApproval
{
    public int PermissionFinanceApprovalId { get; set; }

    public int? ProjectId { get; set; }

    public int? EmpId { get; set; }

    public double Amount { get; set; }

    public virtual EmployeeDetail? Emp { get; set; }

    public virtual Project? Project { get; set; }
}
