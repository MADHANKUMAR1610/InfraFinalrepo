using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Master
{
    public class DepartmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DepartmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<DepartmentDto>> GetDepartments()
        {
            return await _unitOfWork.DepartmentRepository.GetDepartments();
        }
        public async Task<DepartmentWithRoleDto> GetDepartmentWithRoles(int deptId)
        {
            return await _unitOfWork.DepartmentRepository.GetDepartmentWithRoles(deptId);
        }
    }
}
