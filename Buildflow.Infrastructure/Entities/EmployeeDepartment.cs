using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class EmployeeDepartment
{
    public int EmpDeptId { get; set; }

    public int EmpId { get; set; }

    public int DeptId { get; set; }

    public DateOnly? AssignedDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsPrimary { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual Department Dept { get; set; } = null!;

    public virtual EmployeeDetail Emp { get; set; } = null!;
}
