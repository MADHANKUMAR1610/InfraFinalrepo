using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;

using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;




//using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static Buildflow.Utility.DTO.LoginVendorFullDetailDto;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;



namespace Buildflow.Library.Repository
{
    public class RegisterRepository : GenericRepository<EmployeeDetail>, IRegisterRepository
    {

        private readonly ILogger<GenericRepository<Infrastructure.Entities.EmployeeDetail>> _logger;

        private readonly IConfiguration _configuration;
        // Constructor with context and logger injection
        public RegisterRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<Infrastructure.Entities.EmployeeDetail>> logger)
            : base(context, logger)  // Passing the logger to the base class
        {
            _logger = logger;
            _configuration = configuration;
        }
        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<LoginVendorFullDetailDto?> GetLoginVendorFullDetailAsync(int vendorId)
        {
            try
            {
                using var connection = Context.Database.GetDbConnection();
                await connection.OpenAsync();

                // Call the function get_vendor_detail
                const string query = "SELECT * FROM vendor.get_vendor_detail(@vendorid)";
                var result = (await connection.QueryAsync<dynamic>(query, new { vendorid = vendorId })).ToList();

                if (!result.Any())
                    return null;

                var first = result.First();

                return new LoginVendorFullDetailDto
                {
                    VendorId = first.vendor_id ?? 0, // Handle nulls by setting a default value
                    VendorCode = first.vendor_code ?? string.Empty,
                    VendorName = first.vendor_name ?? string.Empty,
                    OrganizationName = first.organization_name ?? string.Empty,
                    Mobile = first.mobile ?? string.Empty,
                    AlternativeMobile = first.alternative_mobile ?? string.Empty,
                    Email = first.email ?? string.Empty,
                    Website = first.website ?? string.Empty,
                    Street = first.street ?? string.Empty,
                    City = first.city ?? string.Empty,
                    State = first.state ?? string.Empty,
                    Country = first.country ?? string.Empty,
                    PostalCode = first.postal_code ?? string.Empty,
                    TaxId = first.tax_id ?? string.Empty,
                    PaymentTerms = first.payment_terms ?? string.Empty,
                    PreferredPaymentMethod = first.preferred_payment_method ?? string.Empty,
                    DeliveryTerms = first.delivery_terms ?? string.Empty,
                    CreditLimit = first.credit_limit ?? null, // Handle nulls by assigning null to nullable type
                    IsActive = first.is_active ?? false, // Default to false if null
                    CreatedAt = first.created_at ?? DateTime.MinValue, // Default to min date if null
                    UpdatedAt = first.updated_at ?? DateTime.MinValue, // Default to min date if null
                    CreatedBy = first.created_by ?? 0, // Default to 0 if null
                    UpdatedBy = first.updated_by ?? 0, // Default to 0 if null
                    Password = first.password ?? string.Empty,
                    RoleId = first.role_id ?? 0,
                    RoleName = first.role_name ?? string.Empty,
                    RoleCode=first.rolecode ?? string.Empty,
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task<LoginEmployeeFullDetailDto?> GetLoginEmployeeFullDetailAsync(int empId)
        {
            try
            {
                using var connection = Context.Database.GetDbConnection();
                await connection.OpenAsync();

                const string query = "SELECT * FROM employee.get_employee_full_detail2(@empid)";

                var result = (await connection.QueryAsync<dynamic>(query, new { empid = empId })).ToList();

                if (!result.Any())
                    return null;

                var firstRecord = result.First();

                // Check if board_id is actually null - this is likely the issue
                int? boardId = firstRecord.board_id;
                string boardName = firstRecord.board_name;

                var employeeDetails = new LoginEmployeeFullDetailDto
                {
                    EmpId = firstRecord.emp_id,
                    EmployeeCode = firstRecord.employee_code,
                    FirstName = firstRecord.first_name,
                    MiddleName = firstRecord.middle_name,
                    LastName = firstRecord.last_name,
                    Email = firstRecord.email,
                    Phone = firstRecord.phone,
                    RoleId = firstRecord.role_id,
                    RoleName = firstRecord.role_name,
                    RoleCode= firstRecord.rolecode,
                    DeptId = firstRecord.dept_id,
                    DeptName = firstRecord.dept_name,


                    Board = boardId.HasValue ? new BoardData
                    {
                        BoardId = boardId.Value,
                        BoardName = boardName
                    } : null,


                    BoardLabels = result
                        .Where(r => r.label_id != null)
                        .GroupBy(r => r.label_id)
                        .Select(g => new BoardLabelData
                        {
                            LabelId = g.First().label_id,
                            LabelName = g.First().label_name,
                            BoardId = g.First().board_id
                        }).ToList(),

                    Tickets = result
                        .Where(r => r.ticket_id != null)
                        .Select(r => new TicketDetail
                        {
                            TicketId = r.ticket_id,
                            TicketNo = r.ticket_no,
                            TicketName = r.ticket_name,
                            TicketDescription = r.ticket_description,
                            TicketCreatedDate = r.ticket_created_date,
                            BoardId = r.board_id,
                            BoardName = r.board_name,
                            BoardDescription = r.board_description
                        }).ToList(),

                    Notifications = result
                        .Where(n => n.notification_id != null)
                        .Select(n => new UserNotifications
                        {
                            notificationId = n.notification_id,
                            emp_id = n.notification_emp_id,
                            is_read = n.is_read,
                            message = n.message,
                        }).ToList(),

                    Projects = result
                        .Where(p => p.project_id != null)
                        .GroupBy(p => p.project_id)
                        .Select(g => new ProjectDetails
                        {
                            ProjectId = g.First().project_id,
                            ProjectName = g.First().project_name,
                            ProjectDescription = g.First().project_description,
                            ProjectTypeId = g.First().project_type_id,
                            ProjectTypeName = g.First().project_type_name,
                            ProjectSectorId = g.First().project_sector_id,
                            ProjectSectorName = g.First().project_sector_name,
                            ProjectStatus = g.First().project_status,
                            StartDate = g.First().start_date,
                            EndDate = g.First().end_date,
                            ClientName = g.First().client_name

                        }).ToList()
                };

                // Debug logging to verify data
                Console.WriteLine($"Board ID: {boardId}, Board Name: {boardName}");
                Console.WriteLine($"Board Labels Count: {employeeDetails.BoardLabels.Count}");

                return employeeDetails;
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error fetching employee full detail: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return null;
            }
        }


        public async Task<List<EmployeeByDeptDto>> GetEmployeesByDeptId(int deptId)
        {
            try
            {
                var result = new List<EmployeeByDeptDto>();
                var connection = Context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    // Modified to use the new function 'get_employees_by_department_with_role'
                    command.CommandText = "SELECT * FROM employee.get_employees_by_department_with_role(@dept_id_input)";
                    command.CommandType = CommandType.Text;

                    var param = command.CreateParameter();
                    param.ParameterName = "@dept_id_input";
                    param.Value = deptId;
                    command.Parameters.Add(param);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new EmployeeByDeptDto
                            {
                                EmployeeName = reader["employee_name"]?.ToString() ?? string.Empty,
                                DeptName = reader["dept_name"]?.ToString() ?? string.Empty,
                                EmpId = reader["emp_id"] != DBNull.Value ? Convert.ToInt32(reader["emp_id"]) : 0,
                                RoleId = reader["role_id"] != DBNull.Value ? Convert.ToInt32(reader["role_id"]) : 0,
                                RoleName = reader["role_name"]?.ToString() ?? string.Empty
                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees by department with role");
                throw;
            }
        }




        public async Task<List<EmployeeByRoleDto>> GetEmployeesByRoleId(int roleId)
        {
            try
            {
                var result = new List<EmployeeByRoleDto>();
                var connection = Context.Database.GetDbConnection();

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM employee.get_employees_by_role(@role_id_input)";
                    command.CommandType = CommandType.Text;

                    var param = command.CreateParameter();
                    param.ParameterName = "@role_id_input";
                    param.Value = roleId;
                    command.Parameters.Add(param);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            result.Add(new EmployeeByRoleDto
                            {
                                EmployeeName = reader["employee_name"]?.ToString() ?? string.Empty,
                                RoleName = reader["role_name"]?.ToString() ?? string.Empty,
                                RoleCode = reader["rolecode"]?.ToString() ?? string.Empty,
                                EmpId = reader["emp_id"] != DBNull.Value ? Convert.ToInt32(reader["emp_id"]) : 0

                            });
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees by role");
                throw;
            }
        }


        //public async Task<EmployeeDataList?> GetEmployeesGroupedByRole()
        //{
        //    try
        //    {
        //        var rolesWithEmployees = await Context.EmployeeRoles
        //            .Include(x => x.Role)
        //            .Include(er => er.Emp)
        //            .ToListAsync();

        //        var result = new EmployeeDataList();

        //        foreach (var group in rolesWithEmployees.GroupBy(r => r.Role.RoleName))
        //        {
        //            var employees = group.Select(r => new EmployeeData
        //            {
        //                EmpId = r.Emp.EmpId,
        //                EmployeeCode = r.Emp.EmployeeCode,
        //                EmployeeName = r.Emp.FirstName,
        //                IsAllocated = r.Emp.IsAllocated,    

        //                Role = r.Role.RoleName,
        //            }).ToList();

        //            result.EmployeesByRole[group.Key] = employees;
        //        }

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (optional - use your logger here)
        //        Console.WriteLine($"Error in GetEmployeesGroupedByRole: {ex.Message}");

        //        // Optionally rethrow or return null
        //        return null;
        //    }
        //}


        public async Task<EmployeeDataList?> GetEmployeesGroupedByRole()
        {
            try
            {
                var rolesWithEmployees = await Context.EmployeeRoles
                    .Include(x => x.Role)
                    .Include(er => er.Emp)
                    .ToListAsync();

                var result = new EmployeeDataList();

                foreach (var group in rolesWithEmployees.GroupBy(r => new { r.Role.RoleId, r.Role.RoleName, r.Role.Rolecode }))
                {
                    var employees = group.Select(r => new EmployeeData
                    {
                        EmpId = r.Emp.EmpId,
                        EmployeeCode = r.Emp.EmployeeCode,
                        EmployeeName = r.Emp.FirstName,
                        IsAllocated = r.Emp.IsAllocated,
                        Role = r.Role.RoleName,
                        RoleId = r.Role.RoleId, // Include RoleId
                        Rolecode = r.Role.Rolecode
                    }).ToList();

                    result.EmployeesByRole[group.Key.RoleName] = employees;
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetEmployeesGroupedByRole: {ex.Message}");
                return null;
            }
        }


        public async Task<IEnumerable<VendorDTO>> GetVendors()
        {
            return await Context.Vendors
                .Select(pt => new VendorDTO
                {
                    Id = pt.VendorId,
                    VendorName = pt.VendorName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SubcontractorDTO>> GetSubcontractors()
        {
            return await Context.Subcontractors
                .Select(ps => new SubcontractorDTO
                {
                    Id = ps.SubcontractorId,
                    SubcontractorName = ps.SubcontractorName
                })
                .ToListAsync();
        }
        public async Task<Refreshtoken?> GetRefreshToken(string refreshToken)
        {
            return await Context.Refreshtokens
      .FirstOrDefaultAsync(r => r.Token == refreshToken && r.Isrevoked == false);
        }

        public async Task<object?> GetUserByEmail(string email)
        {
            // Check EmployeeDetails
            var empresult = await Context.EmployeeDetails
                .FirstOrDefaultAsync(e => e.Email == email);
            if (empresult == null)
            {
                return await Context.Vendors
                  .FirstOrDefaultAsync(v => v.Email == email);
            }
            else
            {
                return empresult;
            }
        }

        public async Task<object?> SaveRefreshToken(string email, string refreshToken)
        {
            var token = new Refreshtoken
            {
                Email = email,
                Token = refreshToken,
                Expirydate = DateTime.Now.AddDays(7), // Valid for 7 days
                Isrevoked = false
            };

            Context.Refreshtokens.Add(token);
            await Context.SaveChangesAsync();
            return token;
        }
        public async Task<object?> VerifyLogin(string email, string password, string? type)
        {
            if (string.IsNullOrEmpty(type))
            {
                // Check EmployeeDetails
                return await Context.EmployeeDetails
                    .FirstOrDefaultAsync(e => e.Email == email && e.Password == password);
            }
            else
            {
                // Check Vendors table
                return await Context.Vendors
                    //.FirstOrDefaultAsync(v => v.Email == email && v.Password == password);
                    .FirstOrDefaultAsync(v => v.Email == email);
            }
        }


        //public async Task<List<BoardData>> GetEmployeeBoardsAsync(int empId)
        //{
        //    var boards = new List<BoardData>();

        //    try
        //    {
        //        var connection = Context.Database.GetDbConnection();
        //        var sql = "SELECT * FROM employee.get_employee_boards_with_labels_and_tickets(@EmpId);";

        //        // Ensure the connection is open
        //        if (connection.State == ConnectionState.Closed)
        //            await connection.OpenAsync();

        //        var rawResults = await connection.QueryAsync<dynamic>(sql, new { EmpId = empId });

        //        foreach (var row in rawResults)
        //        {
        //            int boardId = row.board_id;
        //            int labelId = row.label_id;

        //            var board = boards.FirstOrDefault(b => b.BoardId == boardId);
        //            if (board == null)
        //            {
        //                board = new BoardData
        //                {
        //                    BoardId = boardId,
        //                    BoardName = row.board_name,
        //                    BoardDescription = row.board_description,
        //                    Labels = new List<BoardLabelData>()
        //                };
        //                boards.Add(board);
        //            }

        //            var label = board.Labels.FirstOrDefault(l => l.LabelId == labelId);
        //            if (label == null)
        //            {
        //                label = new BoardLabelData
        //                {
        //                    LabelId = labelId,
        //                    LabelName = row.label_name,
        //                    Tickets = new List<TicketDetail>()
        //                };
        //                board.Labels.Add(label);
        //            }

        //            if (row.ticket_id != null)
        //            {
        //                label.Tickets.Add(new TicketDetail
        //                {
        //                    TicketId = row.ticket_id,
        //                    TicketNo = row.ticket_no,
        //                    TicketName = row.ticket_name,
        //                    TicketDescription = row.ticket_description,
        //                    TicketCreatedDate = row.ticket_created_date,
        //                    TicketType = row.ticket_type,
        //                });
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Replace with your actual logger if injected
        //        Console.WriteLine($"Error fetching boards for employee {empId}: {ex.Message}");
        //        // You can also log the full exception: ex.ToString()

        //        // Optionally rethrow or return an empty list
        //        // throw; // uncomment if you want to propagate the error
        //    }

        //    return boards;
        //}


        public async Task<List<BoardData>> GetEmployeeBoardsAsync(int empId, int? roleId = null)
        {
            var boards = new List<BoardData>();

            try
            {
                var connection = Context.Database.GetDbConnection();
                var sql = "SELECT * FROM employee.get_employee_boards_with_labels_and_tickets_with_vendor(@EmpId, @RoleId);";

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                var rawResults = await connection.QueryAsync<dynamic>(sql, new { EmpId = empId, RoleId = roleId });

                foreach (var row in rawResults)
                {
                    int boardId = row.board_id;
                    int labelId = row.label_id;

                    var board = boards.FirstOrDefault(b => b.BoardId == boardId);
                    if (board == null)
                    {
                        board = new BoardData
                        {
                            BoardId = boardId,
                            BoardName = row.board_name,
                            BoardDescription = row.board_description,
                            Labels = new List<BoardLabelData>()
                        };
                        boards.Add(board);
                    }

                    var label = board.Labels.FirstOrDefault(l => l.LabelId == labelId);
                    if (label == null)
                    {
                        label = new BoardLabelData
                        {
                            LabelId = labelId,
                            LabelName = row.label_name,
                            Tickets = new List<TicketDetail>()
                        };
                        board.Labels.Add(label);
                    }

                    if (row.ticket_id != null)
                    {
                        label.Tickets.Add(new TicketDetail
                        {
                            TicketId = row.ticket_id,
                            TicketNo = row.ticket_no,
                            TicketName = row.ticket_name,
                            TicketDescription = row.ticket_description,
                            TicketCreatedDate = row.ticket_created_date,
                            TicketType = row.ticket_type,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching boards for employee {empId}: {ex}");
            }

            return boards;
        }

        public async Task<List<BoardLabelData>> GetLabelsWithTicketsByEmployeeIdAsync(int empId)
        {
            var labels = new List<BoardLabelData>();

            try
            {
                var connection = Context.Database.GetDbConnection();
                var sql = "SELECT * FROM employee.get_employee_labels_with_tickets(@EmpId);";

                if (connection.State == ConnectionState.Closed)
                    await connection.OpenAsync();

                var rawResults = await connection.QueryAsync<dynamic>(sql, new { EmpId = empId });

                foreach (var row in rawResults)
                {
                    int labelId = row.label_id;

                    var label = labels.FirstOrDefault(l => l.LabelId == labelId);
                    if (label == null)
                    {
                        label = new BoardLabelData
                        {
                            LabelId = labelId,
                            LabelName = row.label_name,
                            Tickets = new List<TicketDetail>()
                        };
                        labels.Add(label);
                    }

                    // Handle tickets if available
                    if (row.ticket_id != null)
                    {
                        label.Tickets.Add(new TicketDetail
                        {
                            TicketId = row.ticket_id,
                            TicketNo = row.ticket_no,
                            TicketName = row.ticket_name,
                            TicketDescription = row.ticket_description,
                            TicketCreatedDate = row.ticket_created_date
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching labels for employee {empId}: {ex.Message}");
                // Optionally rethrow or handle error
                // throw;
            }

            return labels;
        }


    }
}
