using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Employee
{
    public class EmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<EmployeeDTO>> CreateEmployeeAsync(EmployeeDTO input)
        {
            return await _unitOfWork.EmployeeRepository.CreateEmployeeAsync(input);
        }
        public async Task<List<GetEmployeeDto>> GetEmployees()
        {
            return await _unitOfWork.EmployeeRepository.GetEmployees();
        }

        public async Task DeleteEmployee(int empId)
        {
            await _unitOfWork.EmployeeRepository.DeleteEmployeeAsync(empId);
        }

        public async Task<List<EmployeeDTO>> CreateOrUpdateEmployeeAsync(EmployeeDTO input)
        {
            return await _unitOfWork.EmployeeRepository.CreateOrUpdateEmployeeAsync(input);
        }

    }
}
