using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data.Common;
using Microsoft.AspNetCore.Http;

namespace Buildflow.Library.Repository
{
    public class ProjectRepository : GenericRepository<Project>, IProjectRepository
    {

        private readonly ILogger<GenericRepository<Infrastructure.Entities.Project>> _logger;

        private readonly IConfiguration _configuration;

        public ProjectRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<Infrastructure.Entities.Project>> logger)
            : base(context, logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));


        public async Task DeleteProjectCascadeAsync(int projectId)
        {
            try
            {
                var sql = "CALL project.delete_project_cascade(@p_project_id)";
                var param = new Npgsql.NpgsqlParameter("@p_project_id", projectId);

                await Context.Database.ExecuteSqlRawAsync(sql, param);
            }
            catch (Exception ex)
            {

                throw new ApplicationException($"Failed to delete project {projectId}");
            }
        }


        public async Task<BoqDetailsFullDto?> GetBoqDetailsAsync(int boqId)
        {
            const string sql = "SELECT * FROM project.get_boq_details(@p_boq_id);";

            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_boq_id", boqId, DbType.Int32);

            var result = await connection.QueryAsync(sql, parameters);

            if (!result.Any())
                return null;

            var grouped = result.GroupBy(r => new
            {
                boq_id = (int)r.boq_id,
                boq_name = (string)r.boq_name,
                boq_code = (string)r.boq_code,
                project_id = (int)r.project_id,
                project_name = (string)r.project_name,
                created_by = (int)r.created_by,
                vendor_id = r.vendor_id != null ? (int)r.vendor_id : 0,
                vendor_name = (string)r.vendor_name,
                ticket_id = (int)r.ticket_id,
                approvers = r.approvers != null ? (string)r.approvers : "[]"
            }).Select(g => new BoqDetailsFullDto
            {
                TicketId = g.Key.ticket_id,
                BoqId = g.Key.boq_id,
                BoqName = g.Key.boq_name,
                BoqCode = g.Key.boq_code,
                ProjectId = g.Key.project_id,
                ProjectName = g.Key.project_name,
                CreatedBy = g.Key.created_by,
                VendorId = g.Key.vendor_id,
                VendorName = g.Key.vendor_name,
                BoqItems = g.Select(x => new BoqItemDetail
                {
                    BoqItemsId = x.boq_items_id != null ? (int)x.boq_items_id : 0,
                    ItemName = x.item_name != null ? (string)x.item_name : string.Empty,
                    Quantity = x.quantity != null ? (int)x.quantity : 0,
                    Unit = x.unit != null ? (string)x.unit : string.Empty,
                    Price = x.price != null ? (decimal)x.price : 0,
                    Total = x.total != null ? Convert.ToDouble(x.total) : 0.0
                }).ToList(),
                Approvers = System.Text.Json.JsonSerializer.Deserialize<List<ApproverDto>>(g.Key.approvers)
            }).FirstOrDefault();

            return grouped;
        }

        //public async Task<BoqDetailsFullDto?> GetBoqDetailsAsync(int boqId)
        //{
        //    const string sql = "SELECT * FROM project.get_boq_details(@p_boq_id);";

        //    using var connection = CreateConnection();
        //    var parameters = new DynamicParameters();
        //    parameters.Add("p_boq_id", boqId, DbType.Int32);

        //    var result = await connection.QueryAsync(sql, parameters);

        //    if (!result.Any())
        //        return null;

        //    var grouped = result.GroupBy(r => new
        //    {
        //        boq_id = (int)r.boq_id,
        //        boq_name = (string)r.boq_name,
        //        boq_code = (string)r.boq_code,
        //        project_id = (int)r.project_id,
        //        project_name = (string)r.project_name,
        //        created_by = (int)r.created_by,

        //        vendor_id = r.vendor_id != null ? (int)r.vendor_id : 0,

        //        vendor_name = (string)r.vendor_name,
        //        ticket_id = (int)r.ticket_id
        //    }).Select(g => new BoqDetailsFullDto
        //    {
        //        TicketId = g.Key.ticket_id,
        //        BoqId = g.Key.boq_id,
        //        BoqName = g.Key.boq_name,
        //        BoqCode = g.Key.boq_code,
        //        ProjectId = g.Key.project_id,
        //        ProjectName = g.Key.project_name,
        //        CreatedBy = g.Key.created_by,
        //        VendorId = g.Key.vendor_id,
        //        VendorName = g.Key.vendor_name,
        //        BoqItems = g.Select(x => new BoqItemDetail
        //        {
        //            BoqItemsId = x.boq_items_id != null ? (int)x.boq_items_id : 0,
        //            ItemName = x.item_name != null ? (string)x.item_name : string.Empty,
        //            Quantity = x.quantity != null ? (int)x.quantity : 0,
        //            Unit = x.unit != null ? (string)x.unit : string.Empty,
        //            Price = x.price != null ? (decimal)x.price : 0,
        //            Total = x.total != null ? Convert.ToDouble(x.total) : 0.0
        //        }).ToList()
        //    }).FirstOrDefault();

        //    return grouped;
        //}


        public async Task<List<BoqItemsDto>> GetBoqItemsByProjectIdAsync(int projectId)
        {
            var connection = Context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM project.get_boq_items_by_project_id(@p_project_id)";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "p_project_id";
            parameter.Value = projectId;
            parameter.DbType = DbType.Int32;
            command.Parameters.Add(parameter);

            using var reader = await command.ExecuteReaderAsync();

            var boqItems = new List<BoqItemsDto>();

            while (await reader.ReadAsync())
            {
                var boqItem = new BoqItemsDto
                {
                    BoqId = reader.IsDBNull(0) ? null : reader.GetInt32(0),
                    BoqItemsId = reader.IsDBNull(1) ? null : reader.GetInt32(1),
                    ItemName = reader.IsDBNull(2) ? null : reader.GetString(2),
                    Unit = reader.IsDBNull(3) ? null : reader.GetString(3),
                    Price = reader.IsDBNull(4) ? null : reader.GetDecimal(4),
                    Quantity = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                    Total = reader.IsDBNull(6) ? null : reader.GetDouble(6),
                    ApprovalStatus=reader.IsDBNull(7) ? null: reader.GetString(7)
                };

                boqItems.Add(boqItem);
            }

            await connection.CloseAsync();
            return boqItems;
        }
    

public async Task<BoqDetailsFullDto?> GetBoqDetailsAsync(string code)
        {
            const string sql = "SELECT * FROM project.get_boq_details_by_boq_code(@p_boq_code);";

            using var connection = CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("p_boq_code", code, DbType.String);

            var result = await connection.QueryAsync(sql, parameters);

            if (!result.Any())
                return null;

            var grouped = result.GroupBy(r => new
            {
                boq_id = (int)r.boq_id,
                boq_name = (string)r.boq_name,
                boq_code = (string)r.boq_code,
                project_id = (int)r.project_id,
                project_name = (string)r.project_name,
                created_by = (int)r.created_by,
                vendor_id = (int)r.vendor_id,
                vendor_name = (string)r.vendor_name
            }).Select(g => new BoqDetailsFullDto
            {
                BoqId = g.Key.boq_id,
                BoqName = g.Key.boq_name,
                BoqCode = g.Key.boq_code,
                ProjectId = g.Key.project_id,
                ProjectName = g.Key.project_name,
                CreatedBy = g.Key.created_by,
                VendorId = g.Key.vendor_id,
                VendorName = g.Key.vendor_name,
                BoqItems = g.Select(x => new BoqItemDetail
                {
                    BoqItemsId = x.boq_items_id != null ? (int)x.boq_items_id : 0,
                    ItemName = x.item_name != null ? (string)x.item_name : string.Empty,
                    Quantity = x.quantity != null ? (int)x.quantity : 0,
                    Unit = x.unit != null ? (string)x.unit : string.Empty,
                    Price = x.price != null ? (decimal)x.price : 0,
                    Total = x.total != null ? Convert.ToDouble(x.total) : 0.0
                }).ToList()
            }).FirstOrDefault();

            return grouped;
        }

        public async Task<BaseResponse> UpsertProjectAsync(ProjectInput dto)
        {
            var response = new BaseResponse();

            try
            {
                Project project;

                if (dto.ProjectId > 0)
                {
                    // Update existing project
                    project = await Context.Projects.FindAsync(dto.ProjectId);
                    if (project == null)
                    {
                        response.Success = false;
                        response.Message = "Project not found.";
                        return response;
                    }

                    project.ProjectName = dto.ProjectName;
                    project.ProjectLocation = dto.ProjectLocation;
                    project.ProjectTypeId = dto.ProjectTypeId;
                    project.ProjectSectorId = dto.ProjectSectorId;
                    project.ProjectStartDate = dto.ProjectStartDate;
                    project.ProjectEndDate = dto.ExpectedCompletionDate;
                    project.ProjectDescription = dto.Description;
                    project.UpdatedAt = DateTime.UtcNow;

                    Context.Projects.Update(project);
                    response.Message = "Project updated successfully.";
                }
                else
                {
                    // Insert new project
                    project = new Project
                    {
                        ProjectName = dto.ProjectName,
                        ProjectLocation = dto.ProjectLocation,
                        ProjectTypeId = dto.ProjectTypeId,
                        ProjectSectorId = dto.ProjectSectorId,
                        ProjectStartDate = dto.ProjectStartDate,
                        ProjectEndDate = dto.ExpectedCompletionDate,
                        ProjectDescription = dto.Description,
                        ProjectStatus = ProjectStatus.NotApproved.ToString(),
                        ProjectTotalBudget = 0.0m,
                        CreatedAt = DateTime.UtcNow
                    };

                    await Add(project); // From GenericRepository
                    await Context.SaveChangesAsync(); // Get auto-generated ProjectId

                    project.ProjectCode = "P" + project.ProjectId;
                    Context.Projects.Update(project);

                    response.Message = "Project inserted successfully.";
                }

                await Context.SaveChangesAsync();

                response.Success = true;
                response.Data = new { projectid = project.ProjectId };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in UpsertProjectAsync: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while saving the project";
                response.Data = 0;
            }

            return response;
        }


        public async Task<IEnumerable<ProjectTypeDTO>> GetProjectTypesAsync()
        {
            return await Context.ProjectTypes
                .Select(pt => new ProjectTypeDTO
                {
                    Id = pt.ProjectTypeId,
                    ProjectTypeName = pt.ProjectTypeName
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ProjectSectorDTO>> GetProjectSectorsAsync()
        {
            return await Context.ProjectSectors
                .Select(ps => new ProjectSectorDTO
                {
                    Id = ps.ProjectSectorId,
                    ProjectSectorName = ps.ProjectSectorName
                })
                .ToListAsync();
        }

        //public async Task<bool> UpsertProjectBudgetDetails(ProjectBudgetInputDto dto)
        //{
        //    using var con = CreateConnection();
        //    try
        //    {
        //        var parameters = new DynamicParameters();
        //        parameters.Add("project_id", dto.ProjectId, DbType.Int64);
        //        parameters.Add("budget_data", JsonConvert.SerializeObject(dto.ProjectBudgetList), DbType.String);
        //        await con.ExecuteAsync("CALL project.insert_project_budget_details(@project_id,@budget_data)", parameters);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("Error inserting project budget details: " + ex.Message);
        //        throw;
        //    }
        //}


        //project Repository 

        public async Task<BaseResponse> UpsertBoqAsync(UpsertBoqRequestDto dto)
        {
            using var con = CreateConnection();
            var response = new BaseResponse();

            try
            {
                var parameters = new DynamicParameters();

                parameters.Add("p_emp_id", dto.EmpId, DbType.Int32);
                parameters.Add("p_boq_id", dto.BoqId ?? 0, DbType.Int32); // Default to 0 if null
                parameters.Add("p_boq_name", dto.BoqName, DbType.String);
                parameters.Add("p_boq_code", dto.BoqCode, DbType.String);
                //parameters.Add("p_boq_items_data", JsonConvert.SerializeObject(dto.BoqItems), DbType.String); // JSONB as string
                //parameters.Add("p_boq_items_data", JsonConvert.SerializeObject(dto.BoqItems), DbType.Object);
                var formattedItems = dto.BoqItems.Select(x => new {
    boq_items_id = x.BoqItemsId,
    item_name = x.ItemName,
    unit = x.Unit,
    price = x.Price,
    quantity = x.Quantity
});

parameters.Add("p_boq_items_data", JsonConvert.SerializeObject(formattedItems), DbType.Object);

                parameters.Add("p_assign_to", dto.AssignTo.ToArray(), DbType.Object); // integer[]
                parameters.Add("p_ticket_type", dto.TicketType, DbType.String);
                parameters.Add("p_vendor_id", dto.VendorId, DbType.Int32);

                // INOUT parameters
                parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
                parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 20);
                parameters.Add("out_boq_id", dbType: DbType.Int32, direction: ParameterDirection.InputOutput);

                // Call stored procedure
                await con.ExecuteAsync(@"
     CALL project.upsert_boq(
         @p_emp_id, 
         @p_boq_id, 
         @p_boq_name, 
         @p_boq_code, 
         
         @p_boq_items_data::jsonb, 
         @p_assign_to, 
         @p_ticket_type, 
         @p_vendor_id, 
         @message, 
         @status, 
         @out_boq_id
     );", parameters);

                var status = parameters.Get<string>("status");
                var message = parameters.Get<string>("message");
                var outBoqId = parameters.Get<int>("out_boq_id");

                response.Success = status?.ToLower() == "success";
                response.Message = message;
                response.Data = new { BoqId = outBoqId };
            }
            catch (Exception ex)
            {
                _logger.LogError("❌ Error in UpsertBoqAsync: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while saving BOQ data";
                response.Data = false;
            }

            return response;
        }
        public async Task<BaseResponse> UpsertProjectBudgetDetails(ProjectBudgetInputDto dto)
        {
            using var con = CreateConnection();
            var response = new BaseResponse();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("iproject_id", dto.ProjectId, DbType.Int32); // match the function signature
                parameters.Add("itotal_cost", dto.TotalCost, DbType.Decimal); // match the function signature
                parameters.Add("budget_data", JsonConvert.SerializeObject(dto.ProjectBudgetList), DbType.String);

                // Add INOUT parameters
                parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
                parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 10);

                // Call the stored procedure
                await con.ExecuteAsync("CALL project.upsert_project_budget(@iproject_id,@itotal_cost,@budget_data, @message, @status);", parameters);

                // Retrieve the output values for message and status
                var status = parameters.Get<string>("status");
                var message = parameters.Get<string>("message");

                // Set the response based on the status
                response.Success = status?.ToLower() == "true";
                response.Message = message;

                // Set the response data with the project id
                response.Data = new { projectid = dto.ProjectId };  // Use the project id from the DTO
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting project budget details: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while inserting budget details";
                response.Data = false;
            }

            return response;
        }

        public async Task<BaseResponse> UpsertProjectMilestoneDetails(ProjectMilestoneInputDto dto)
        {
            using var con = CreateConnection();
            var response = new BaseResponse();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("iproject_id", dto.ProjectId, DbType.Int32);
                parameters.Add("milestone_data", JsonConvert.SerializeObject(dto.MilestoneList), DbType.String);

                // Output parameters
                parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
                parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 10);

                await con.ExecuteAsync("CALL project.insert_project_milestones(@iproject_id, @milestone_data, @message, @status);", parameters);

                var status = parameters.Get<string>("status");
                var message = parameters.Get<string>("message");

                response.Success = status?.ToLower() == "true";
                response.Message = message;
                response.Data = new { projectid = dto.ProjectId };
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting project milestone details: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while inserting milestone details";
                response.Data = false;
            }

            return response;
        }


        public async Task<BaseResponse> UpsertProjectTeam(ProjectTeamInputDto dto)
        {
            using var con = CreateConnection();
            var response = new BaseResponse();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("iproject_id", dto.ProjectId, DbType.Int32);
                parameters.Add("project_team_data", JsonConvert.SerializeObject(dto.TeamList), DbType.String);

                response = await con.QueryFirstAsync<BaseResponse>(
                    "project.upsert_project_team",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting project team: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while inserting team members";
                response.Data = false;
            }

            return response;
        }



        public async Task<(bool Success, string Message, object Data)> UpsertProjectTeamAsync(ProjectTeamUpsertDto dto)
        {
            try
            {
                // Check if connection is open and open it if needed
                var conn = Context.Database.GetDbConnection();
                if (conn.State != ConnectionState.Open)
                    await conn.OpenAsync();

                // Create command
                await using var command = conn.CreateCommand();
                command.CommandText = "project.upsert_project_team_with_status_update";
                command.CommandType = CommandType.StoredProcedure;

                // Helper function to add array parameters safely
                void AddArrayParameter(string name, IEnumerable<int> values)
                {
                    var parameter = new NpgsqlParameter(name, NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer);
                    parameter.Value = values?.ToArray() ?? Array.Empty<int>();
                    command.Parameters.Add(parameter);
                }

                // Add input parameters
                command.Parameters.Add(new NpgsqlParameter("p_project_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.ProjectId });

                // Add array parameters
                AddArrayParameter("p_pm_id", dto.PmId);
                AddArrayParameter("p_apm_id", dto.ApmId);
                AddArrayParameter("p_lead_engg_id", dto.LeadEnggId);
                AddArrayParameter("p_site_supervisor_id", dto.SiteSupervisorId);
                AddArrayParameter("p_qs_id", dto.QsId);
                AddArrayParameter("p_aqs_id", dto.AqsId);
                AddArrayParameter("p_site_engg_id", dto.SiteEnggId);
                AddArrayParameter("p_engg_id", dto.EnggId);
                AddArrayParameter("p_designer_id", dto.DesignerId);
                AddArrayParameter("p_vendor_id", dto.VendorId);
                AddArrayParameter("p_subcontractor_id", dto.SubcontractorId);

                // Add output parameters
                var successParam = new NpgsqlParameter("success", NpgsqlTypes.NpgsqlDbType.Boolean)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = false
                };
                var messageParam = new NpgsqlParameter("message", NpgsqlTypes.NpgsqlDbType.Text)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = string.Empty
                };
                var resultProjectIdParam = new NpgsqlParameter("result_project_id", NpgsqlTypes.NpgsqlDbType.Integer)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = DBNull.Value
                };

                command.Parameters.Add(successParam);
                command.Parameters.Add(messageParam);
                command.Parameters.Add(resultProjectIdParam);

                // Execute command
                await command.ExecuteNonQueryAsync();

                // Get output values
                var success = (bool)successParam.Value;
                var message = messageParam.Value?.ToString() ?? "No message returned";
                var projectId = resultProjectIdParam.Value != DBNull.Value ? (int?)resultProjectIdParam.Value : null;

                return (success, message, new { ProjectId = projectId });
            }
            catch (Exception ex)
            {
                // Enhanced error information
                string errorDetails = $"Exception in UpsertProjectTeamAsync";
                if (ex.InnerException != null)
                    errorDetails += $" | Inner exception: {ex.InnerException.Message}";

                // Log the error (assuming you have logging configured)
                // _logger.LogError(ex, errorDetails);

                return (false, errorDetails, null);
            }
        }


        //public async Task<(bool Success, string Message, object Data)> UpsertProjectTeamAsync(ProjectTeamUpsertDto dto)
        //{
        //    try
        //    {
        //        var conn = Context.Database.GetDbConnection();

        //        await using var command = conn.CreateCommand();
        //        command.CommandText = "project.upsert_project_team";
        //        command.CommandType = CommandType.StoredProcedure;

        //        command.Parameters.Add(new NpgsqlParameter("p_project_id", dto.ProjectId));
        //        command.Parameters.Add(new NpgsqlParameter("p_pm_id", dto.PmId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_apm_id", dto.ApmId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_lead_engg_id", dto.LeadEnggId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_site_supervisor_id", dto.SiteSupervisorId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_qs_id", dto.QsId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_aqs_id", dto.AqsId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_site_engg_id", dto.SiteEnggId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_engg_id", dto.EnggId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_designer_id", dto.DesignerId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_vendor_id", dto.VendorId ?? new List<int>()));
        //        command.Parameters.Add(new NpgsqlParameter("p_subcontractor_id", dto.SubcontractorId ?? new List<int>()));

        //        var successParam = new NpgsqlParameter("success", NpgsqlTypes.NpgsqlDbType.Boolean)
        //        {
        //            Direction = ParameterDirection.InputOutput,
        //            Value = false
        //        };
        //        var messageParam = new NpgsqlParameter("message", NpgsqlTypes.NpgsqlDbType.Text)
        //        {
        //            Direction = ParameterDirection.InputOutput,
        //            Value = string.Empty
        //        };
        //        var resultProjectIdParam = new NpgsqlParameter("result_project_id", NpgsqlTypes.NpgsqlDbType.Integer)
        //        {
        //            Direction = ParameterDirection.InputOutput,
        //            Value = DBNull.Value
        //        };

        //        command.Parameters.Add(successParam);
        //        command.Parameters.Add(messageParam);
        //        command.Parameters.Add(resultProjectIdParam);

        //        if (conn.State != ConnectionState.Open)
        //            await conn.OpenAsync();

        //        await command.ExecuteNonQueryAsync();

        //        var success = (bool)successParam.Value;
        //        var message = messageParam.Value.ToString();
        //        var projectId = resultProjectIdParam.Value != DBNull.Value ? (int?)resultProjectIdParam.Value : null;

        //        return (success, message, new { projectId });
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, $"Exception: {ex.Message}", null);
        //    }
        //}


        public async Task<BaseResponse> UpsertProjectPermissionFinanceApproval(ProjectPermissionFinanceApprovalInputDto dto)
        {
            using var con = CreateConnection();
            var response = new BaseResponse();
            try
            {
                foreach (var approval in dto.ProjectPermissionFinanceApprovalList)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("iproject_id", dto.ProjectId, DbType.Int32);
                    parameters.Add("iemp_id", approval.EmpId, DbType.Int32); // Use EmpId from ProjectPermissionFinanceApprovalInput
                    parameters.Add("iamount", approval.Amount, DbType.Int32);

                    response = await con.QueryFirstAsync<BaseResponse>(
                    "project.upsert_permission_finance_approval",
                    parameters, commandType: CommandType.StoredProcedure
                );
                    response.Success = true;
                    response.Data = dto.ProjectId;

                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error inserting project permission finance approval: " + ex.Message);
                response.Success = false;
                response.Message = "An error occurred while inserting project permission finance approval";
                response.Data = false;
            }
            return response;
        }


        public async Task<IActionResult?> GetProjectDetailsByIdAsync(int projectId)
        {
            try
            {
                using var connection = Context.Database.GetDbConnection();
                await connection.OpenAsync();

                const string sql = "SELECT project.get_project_with_budget_details1(@ProjectId)";
                var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { ProjectId = projectId });

                if (string.IsNullOrWhiteSpace(result))
                {
                    return new ObjectResult("No project found with the specified ID.")
                    {
                        StatusCode = 404
                    };
                }

                var parsedJson = System.Text.Json.JsonSerializer.Deserialize<object>(result);
                return new JsonResult(parsedJson)
                {
                    StatusCode = 200,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., with Serilog or ILogger)
                return new ObjectResult($"An error occurred")
                {
                    StatusCode = 500
                };
            }
        }

        //public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        //{
        //    using var connection = Context.Database.GetDbConnection();
        //    var projects = await connection.QueryAsync<ProjectDto>("SELECT * FROM project.get_all_projects()");
        //    return projects;
        //}

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            using var connection = Context.Database.GetDbConnection();
            var sql = "SELECT * FROM project.get_all_projects() ORDER BY project_id DESC;";
            var projects = await connection.QueryAsync<ProjectDto>(sql);
            return projects;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string? status)
        {
            using var connection = Context.Database.GetDbConnection();
            var sql = "SELECT * FROM project.get_all_projects_by_status1(@p_status)";
            var parameters = new { p_status = status };

            // Execute the query using Dapper
            var projects = await connection.QueryAsync<ProjectDto>(sql, parameters);

            return projects;
        }

        public async Task UpsertProjectApprovalAsync(int approvedBy, int ticketId, string approvalType)
        {
            try
            {
                await Context.Database.ExecuteSqlRawAsync(
                    "CALL project.upsert_project_approval(p_approved_by := {0}, p_ticket_id := {1}, p_approval_input := {2})",
                    approvedBy, ticketId, approvalType);
            }
            catch (Exception ex)
            {
                throw new Exception("An unexpected error occurred while inserting project approval");
            }
        }


        //public async Task<IActionResult?> GetProjectTeamDetailsByProjectIdAsync(int projectId)
        //{
        //    try
        //    {
        //        await using var connection = Context.Database.GetDbConnection();
        //        await connection.OpenAsync();

        //        const string sql = "SELECT project.get_project_teamdetails_by_project_id(@ProjectId)";
        //        var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { ProjectId = projectId });

        //        if (string.IsNullOrWhiteSpace(result) || result == "null")
        //        {
        //            return new ObjectResult("No project found with the specified ID.")
        //            {
        //                StatusCode = 404
        //            };
        //        }

        //        var parsedJson = System.Text.Json.JsonSerializer.Deserialize<object>(result);

        //        return new JsonResult(parsedJson)
        //        {
        //            StatusCode = 200,
        //            ContentType = "application/json"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception: ex.ToString(-)
        //        return new ObjectResult("An error occurred while retrieving project team details.")
        //        {
        //            StatusCode = 500
        //        }
        //        ;
        //    }
        //}


        public async Task<IActionResult?> GetProjectTeamDetailsByProjectIdAsync(int projectId)
        {
            try
            {
                using var connection = Context.Database.GetDbConnection();
                await connection.OpenAsync();

                const string sql = "SELECT project.get_project_teamdetails_by_project_id(@ProjectId)";
                var result = await connection.QueryFirstOrDefaultAsync<string>(sql, new { ProjectId = projectId });

                if (string.IsNullOrWhiteSpace(result))
                {
                    return new ObjectResult("No project found with the specified ID.")
                    {
                        StatusCode = 404
                    };
                }

                var parsedJson = System.Text.Json.JsonSerializer.Deserialize<object>(result);
                return new JsonResult(parsedJson)
                {
                    StatusCode = 200,
                    ContentType = "application/json"
                };
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., with Serilog or ILogger)
                return new ObjectResult($"An error occurred")
                {
                    StatusCode = 500
                };
            }
        }

    }
}
