using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class DepartmentRoleMapping
{
    public int Id { get; set; }

    public int DeptId { get; set; }

    public int RoleId { get; set; }

    public virtual Department Dept { get; set; } = null!;

    public virtual Role Role { get; set; } = null!;
}
