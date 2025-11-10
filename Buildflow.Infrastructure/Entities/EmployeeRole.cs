using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class EmployeeRole
{
    public int EmpRoleId { get; set; }

    public int EmpId { get; set; }

    public int RoleId { get; set; }

    public DateOnly? AssignedDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual EmployeeDetail Emp { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
