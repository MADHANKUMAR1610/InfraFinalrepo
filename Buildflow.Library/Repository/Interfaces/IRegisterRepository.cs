using Buildflow.Infrastructure.Entities;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Mvc;




//using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IRegisterRepository : IGenericRepository<EmployeeDetail>
    {
        public Task<object?> VerifyLogin(string email, string password,string? type);
        public Task<object?> SaveRefreshToken(string email, string refreshToken);
        public Task<Refreshtoken?> GetRefreshToken(string refreshToken);
        public Task<object?> GetUserByEmail(string email);
        
        public Task<LoginEmployeeFullDetailDto?> GetLoginEmployeeFullDetailAsync(int empId);
        public Task<LoginVendorFullDetailDto?> GetLoginVendorFullDetailAsync(int empId);
       
        //public Task<IActionResult?> GetLoginEmployeeFullDetailAsync(int empId);
        public Task<EmployeeDataList?> GetEmployeesGroupedByRole();
        Task<List<BoardData>> GetEmployeeBoardsAsync(int empId, int? roleId = null);
        Task<List<BoardLabelData>> GetLabelsWithTicketsByEmployeeIdAsync(int empId);
        Task<IEnumerable<VendorDTO>> GetVendors();
        Task<IEnumerable<SubcontractorDTO>> GetSubcontractors();
        Task<List<EmployeeByRoleDto>> GetEmployeesByRoleId(int roleId);
        Task<List<EmployeeByDeptDto>> GetEmployeesByDeptId(int deptId);

    }
}
