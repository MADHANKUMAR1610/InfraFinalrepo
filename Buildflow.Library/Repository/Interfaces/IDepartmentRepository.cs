using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IDepartmentRepository
    {
        public Task<List<DepartmentDto>> GetDepartments();
        public Task<DepartmentWithRoleDto> GetDepartmentWithRoles(int deptId);
    }
}
