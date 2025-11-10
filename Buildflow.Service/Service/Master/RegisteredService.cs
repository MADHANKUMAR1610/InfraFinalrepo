using Buildflow.Infrastructure.Entities;

using Buildflow.Library.Repository.Interfaces;
using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Mvc;




//using Microsoft.AspNetCore.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Master
{
    public class RegisterService
    {

        private readonly IUnitOfWork _unitOfWork;
        public RegisterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<object?> VerifyLogin(string email, string password, string? type)
        {
            return await _unitOfWork.Employees.VerifyLogin(email, password, type);
        }
        public async Task<object?> GetUserByEmail(string email)
        {
            return await _unitOfWork.Employees.GetUserByEmail(email);
        }

        public async Task<Refreshtoken?> GetRefreshToken(string refreshToken)
        {
            return await _unitOfWork.Employees.GetRefreshToken(refreshToken);
        }
        public async Task<object?> SaveRefreshToken(string email, string refreshToken)
        {
            return await _unitOfWork.Employees.SaveRefreshToken(email, refreshToken);
        }

        public async Task<LoginEmployeeFullDetailDto?> GetLoginEmployeeFullDetailAsync(int empId)
        {
            return await _unitOfWork.LoginEmployee.GetLoginEmployeeFullDetailAsync(empId);
        }
        public async Task<LoginVendorFullDetailDto?> GetLoginVendorFullDetailAsync(int empId)
        {
            return await _unitOfWork.LoginEmployee.GetLoginVendorFullDetailAsync(empId);
        }

      

        //public async Task<IActionResult?> GetLoginEmployeeFullDetailAsync(int empId)
        //{
        //    return await _unitOfWork.LoginEmployee.GetLoginEmployeeFullDetailAsync(empId);
        //}
        public async Task<EmployeeDataList> GetEmployeesByRoles()
        {
            return await _unitOfWork.EmployeeRoles.GetEmployeesGroupedByRole();
        }
        public async Task<List<BoardData>> GetEmployeeBoardsAsync(int empId, int? roleId = null)
        {
            return await _unitOfWork.EmployeeRoles.GetEmployeeBoardsAsync(empId, roleId);
        }

        public async Task<List<EmployeeByRoleDto>> GetEmployeesByRole(int roleId)
        {
            return await _unitOfWork.RegisterUser.GetEmployeesByRoleId(roleId);
        }

        public async Task<List<EmployeeByDeptDto>> GetEmployeesByDept(int deptId)
        {
            return await _unitOfWork.RegisterUser.GetEmployeesByDeptId(deptId);
        }

        public async Task<List<BoardLabelData>> GetLabelsWithTicketsByEmployeeIdAsync(int deptId)
        {
            return await _unitOfWork.RegisterUser.GetLabelsWithTicketsByEmployeeIdAsync(deptId);
        }

        public async Task<VendorAndSubcontractor> GetVendorsAndSubcontractors()
        {
            var vendors = await _unitOfWork.VendorDetails.GetVendors();
            var subcontractors = await _unitOfWork.SubcontractorDetails.GetSubcontractors();

            return new VendorAndSubcontractor
            {
                Vendors = vendors.ToList(),
                Subcontractors = subcontractors.ToList()
            };
        }
    }
}
