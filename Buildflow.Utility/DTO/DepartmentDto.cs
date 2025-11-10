using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class DepartmentDto
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; } = string.Empty;
    }
    public class RolesDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
    }
    public class DepartmentWithRoleDto
    {
        public int DeptId { get; set; }
        public List<RolesDto> Roles { get; set; } = new List<RolesDto>();
    }
}
