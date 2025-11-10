using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IEmployeeRepository
    {

        Task<List<EmployeeDTO>> CreateEmployeeAsync(EmployeeDTO input);
        Task<List<GetEmployeeDto>> GetEmployees();

        Task<List<EmployeeDTO>> CreateOrUpdateEmployeeAsync(EmployeeDTO input);
        Task DeleteEmployeeAsync(int empId);

    }
}
