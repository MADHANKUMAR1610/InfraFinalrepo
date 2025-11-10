using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class EmployeeRepository :  IEmployeeRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenericRepository<EmployeeDetail>> _logger;

        public EmployeeRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<EmployeeDetail>> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }


        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<List<GetEmployeeDto>> GetEmployees()
        {
            var employees = new List<GetEmployeeDto>();

            try
            {
                await using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                await using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM employee.get_all_employees()";
                command.CommandType = CommandType.Text;

                await using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var employee = new GetEmployeeDto
                    {
                        EmpId = reader.GetInt32(reader.GetOrdinal("emp_id")),
                        EmployeeCode = reader.IsDBNull(reader.GetOrdinal("employee_code")) ? "" : reader.GetString(reader.GetOrdinal("employee_code")),
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.IsDBNull(reader.GetOrdinal("last_name")) ? null : reader.GetString(reader.GetOrdinal("last_name")),
                        Phone = reader.IsDBNull(reader.GetOrdinal("contact")) ? null : reader.GetString(reader.GetOrdinal("contact")),
                        IsAllocated = !reader.IsDBNull(reader.GetOrdinal("is_allocated")) && reader.GetBoolean(reader.GetOrdinal("is_allocated")),
                        DeptId = reader.IsDBNull(reader.GetOrdinal("dept_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("dept_id")),
                        DeptName = reader.IsDBNull(reader.GetOrdinal("dept_name")) ? null : reader.GetString(reader.GetOrdinal("dept_name")),
                        Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                        Gender = reader.IsDBNull(reader.GetOrdinal("gender")) ? null : reader.GetString(reader.GetOrdinal("gender")),
                        DateOfBirth = reader.IsDBNull(reader.GetOrdinal("date_of_birth")) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("date_of_birth"))),
                        RoleName = reader.IsDBNull("role_name") ? null : reader.GetString(reader.GetOrdinal("role_name")),
                        RoleId = reader.IsDBNull(reader.GetOrdinal("role_id"))? 0: reader.GetInt32(reader.GetOrdinal("role_id")),
                        RoleCode = reader.IsDBNull(reader.GetOrdinal("rolecode")) ? null : reader.GetString(reader.GetOrdinal("rolecode")),
                    };

                    employees.Add(employee);
                }

                return employees;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching employee data.");
                throw new ApplicationException("Could not fetch employees", ex);
            }
        }
        public async Task DeleteEmployeeAsync(int empId)
        {
            var employee = await _context.EmployeeDetails.FindAsync(empId);

            if (employee == null)
            {
                throw new Exception("Employee not found.");
            }

            _context.EmployeeDetails.Remove(employee);
            await _context.SaveChangesAsync();
        }
        public async Task<List<EmployeeDTO>> CreateEmployeeAsync(EmployeeDTO input)
        {
            try
            {
                var employees = new List<EmployeeDTO>();

                // Map EmployeeDTO to EmployeeDetail entity
                var employeeDetail = new EmployeeDetail
                {
                    DateOfBirth = input.DateOfBirth,
                    FirstName = input.FirstName,
                    MiddleName = input.MiddleName,
                    LastName = input.LastName,
                    Phone = input.Phone,
                    Email = input.Email,
                    Gender = input.Gender,
                    Password="Test@123"
                    // Add other fields if needed
                };

                // Add employee to context and save to get the EmpId
                _context.EmployeeDetails.Add(employeeDetail);
                await _context.SaveChangesAsync();

                // Map the designation to EmpRole (after getting EmpId)
                var employeeRole = new EmployeeRole
                {
                    EmpId = employeeDetail.EmpId, // Use the generated EmpId
                    RoleId = input.Designation
                };
                _context.EmployeeRoles.Add(employeeRole);
                await _context.SaveChangesAsync();


                var employeeDepartment = new EmployeeDepartment
                {
                    EmpId = employeeDetail.EmpId, // Use the generated EmpId
                    DeptId = input.Department
                };
                _context.EmployeeDepartments.Add(employeeDepartment);
                await _context.SaveChangesAsync();

                // Map back to DTO for returning
                var result = _context.EmployeeDetails
                    .Where(e => e.Email == input.Email)
                    .Select(employee => new EmployeeDTO
                    {
                        FirstName = employee.FirstName,
                        MiddleName = employee.MiddleName,
                        LastName = employee.LastName,
                        Gender=employee.Gender,
                        Phone = employee.Phone,
                        Email = employee.Email,
                        DateOfBirth = employee.DateOfBirth,
                        Designation = employeeRole.RoleId,
                        Department = employeeDepartment.DeptId,
                        
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating employees.", ex);
            }
        }

        public async Task<List<EmployeeDTO>> CreateOrUpdateEmployeeAsync(EmployeeDTO input)
        {
            try
            {
                EmployeeDetail employeeDetail;

                // Create or update logic
                if (input.EmpId == 0)
                {
                    // Create new employee
                    employeeDetail = new EmployeeDetail
                    {
                        FirstName = input.FirstName,
                        MiddleName = input.MiddleName,
                        LastName = input.LastName,
                        Phone = input.Phone,
                        Email = input.Email,
                        EmployeeCode = input.EmployeeCode,
                        DateOfBirth = input.DateOfBirth,
                        Gender = input.Gender,
                        Password="Test@123"
                    };

                    _context.EmployeeDetails.Add(employeeDetail);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update existing employee
                    employeeDetail = await _context.EmployeeDetails
                        .FirstOrDefaultAsync(e => e.EmpId == input.EmpId);

                    if (employeeDetail == null)
                        throw new Exception("Employee not found with the given EmpId.");

                    employeeDetail.FirstName = input.FirstName;
                    employeeDetail.MiddleName = input.MiddleName;
                    employeeDetail.LastName = input.LastName;
                    employeeDetail.Phone = input.Phone;
                    employeeDetail.Email = input.Email;
                    employeeDetail.EmployeeCode = input.EmployeeCode;
                    employeeDetail.DateOfBirth = input.DateOfBirth;
                    employeeDetail.Gender = input.Gender;

                    _context.EmployeeDetails.Update(employeeDetail);
                    await _context.SaveChangesAsync();
                }

                // Role
                var employeeRole = await _context.EmployeeRoles
                    .FirstOrDefaultAsync(er => er.EmpId == employeeDetail.EmpId);

                if (employeeRole == null)
                {
                    employeeRole = new EmployeeRole
                    {
                        EmpId = employeeDetail.EmpId,
                        RoleId = input.Designation
                    };
                    _context.EmployeeRoles.Add(employeeRole);
                }
                else
                {
                    employeeRole.RoleId = input.Designation;
                    _context.EmployeeRoles.Update(employeeRole);
                }

                // Department
                var employeeDepartment = await _context.EmployeeDepartments
                    .FirstOrDefaultAsync(ed => ed.EmpId == employeeDetail.EmpId);

                if (employeeDepartment == null)
                {
                    employeeDepartment = new EmployeeDepartment
                    {
                        EmpId = employeeDetail.EmpId,
                        DeptId = input.Department
                    };
                    _context.EmployeeDepartments.Add(employeeDepartment);
                }
                else
                {
                    employeeDepartment.DeptId = input.Department;
                    _context.EmployeeDepartments.Update(employeeDepartment);
                }

                await _context.SaveChangesAsync();

                // Return as a list
                return new List<EmployeeDTO>
 {
     new EmployeeDTO
     {
         EmpId = employeeDetail.EmpId,
         FirstName = employeeDetail.FirstName,
         MiddleName = employeeDetail.MiddleName,
         LastName = employeeDetail.LastName,
         Phone = employeeDetail.Phone,
         Email = employeeDetail.Email,
         EmployeeCode = employeeDetail.EmployeeCode,
         DateOfBirth = employeeDetail.DateOfBirth,
         Designation = employeeRole.RoleId,
         Department = employeeDepartment.DeptId,
         Gender=employeeDetail.Gender
     }
 };
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating or updating the employee.", ex);
            }
        }
    }
}



