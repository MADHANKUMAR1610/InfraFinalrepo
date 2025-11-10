using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {

        private readonly ILogger<GenericRepository<Department>> _logger;


        // Constructor with context and logger injection
        public DepartmentRepository(BuildflowAppContext context, ILogger<GenericRepository<Department>> logger)
            : base(context, logger)  // Passing the logger to the base class
        {
            _logger = logger;
        }



        public async Task<List<DepartmentDto>> GetDepartments()
        {
            try
            {
                // Directly execute raw SQL and map the results to RoleDto
                var roles = await Context.Departments
                    .FromSqlRaw("SELECT * FROM master.get_all_departments()")
                    .Select(r => new DepartmentDto
                    {
                        DeptId = r.DeptId,
                        DeptName = r.DeptName,
                        
                    })
                    .ToListAsync();

                return roles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching roles.");
                throw new ApplicationException("Could not fetch roles", ex);
            }
        }




        public async Task<DepartmentWithRoleDto?> GetDepartmentWithRoles(int deptId)
        {
            var dto = new DepartmentWithRoleDto { DeptId = deptId };

            try
            {
                using var connection = Context.Database.GetDbConnection();
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM master.get_roles_by_department(@p_dept_id)";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "p_dept_id";
                parameter.Value = deptId;
                command.Parameters.Add(parameter);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    dto.Roles.Add(new RolesDto
                    {
                        RoleId = reader.GetInt32(reader.GetOrdinal("role_id")),
                        RoleName = reader.GetString(reader.GetOrdinal("role_name")),
                        RoleCode = reader.IsDBNull(reader.GetOrdinal("rolecode")) ? null : reader.GetString(reader.GetOrdinal("rolecode"))
                    });
                }

                return dto.Roles.Any() ? dto : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching department roles.");
                throw new ApplicationException("Could not fetch department roles", ex);
            }
        }



    }
}
