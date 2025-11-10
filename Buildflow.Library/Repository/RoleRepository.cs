using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class RoleRepository : GenericRepository<Role>, IRoleRepository
    {

        private readonly ILogger<GenericRepository<Role>> _logger;


        // Constructor with context and logger injection
        public RoleRepository(BuildflowAppContext context, ILogger<GenericRepository<Role>> logger)
            : base(context, logger)  // Passing the logger to the base class
        {
            _logger = logger;
        }



        public async Task<List<RoleDto>> GetRoles()
        {
            try
            {
                // Directly execute raw SQL and map the results to RoleDto
                var roles = await Context.Roles
                    .FromSqlRaw("SELECT * FROM master.get_all_roles()")
                    .Select(r => new RoleDto
                    {
                        RoleId = r.RoleId,
                        RoleName = r.RoleName,
                        Rolecode = r.Rolecode,
                        RoleDescription = r.RoleDescription,
                        IsSystemRole = r.IsSystemRole.Value,
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





    }
}
