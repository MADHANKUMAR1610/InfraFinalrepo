using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Role
{
    public int RoleId { get; set; }

    public string RoleName { get; set; } = null!;

    public string? RoleDescription { get; set; }

    public bool? IsSystemRole { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? Rolecode { get; set; }

    public virtual ICollection<DepartmentRoleMapping> DepartmentRoleMappings { get; set; } = new List<DepartmentRoleMapping>();

    public virtual ICollection<EmployeeRole> EmployeeRoles { get; set; } = new List<EmployeeRole>();

    public virtual ICollection<Vendorrole> Vendorroles { get; set; } = new List<Vendorrole>();
}
