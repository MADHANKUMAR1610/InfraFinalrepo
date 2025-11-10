using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Department
{
    public int DeptId { get; set; }

    public string DeptName { get; set; } = null!;

    public string? DeptDescription { get; set; }

    public string? DeptCode { get; set; }

    public int? ParentDeptId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public virtual ICollection<DepartmentRoleMapping> DepartmentRoleMappings { get; set; } = new List<DepartmentRoleMapping>();

    public virtual ICollection<EmployeeDepartment> EmployeeDepartments { get; set; } = new List<EmployeeDepartment>();

    public virtual ICollection<Department> InverseParentDept { get; set; } = new List<Department>();

    public virtual Department? ParentDept { get; set; }

    public virtual ICollection<VendorDepartment> VendorDepartments { get; set; } = new List<VendorDepartment>();
}
