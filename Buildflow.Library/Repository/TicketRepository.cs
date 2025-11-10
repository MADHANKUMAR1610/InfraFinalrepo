using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        private readonly ILogger<GenericRepository<Infrastructure.Entities.Ticket>> _logger;

        private readonly IConfiguration _configuration;

        public TicketRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<Infrastructure.Entities.Ticket>> logger)
            : base(context, logger)
        {
            _logger = logger;
            _configuration = configuration;
        }


        string baseUrl = "  https://buildflowtestingapi.crestclimbers.com";

        //string baseUrl = "https://buildflowgraphql.crestclimbers.com";
        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        public async Task<(int TicketId, string Message, string Status)> UpdateTicketById(UpdateTicketDto dto)
        {
            NpgsqlConnection? connection = null;
            try
            {
                connection = (NpgsqlConnection?)CreateConnection();
                await connection.OpenAsync();

                //var command = new NpgsqlCommand("CALL ticket.update_ticket_sp(@p_ticket_id, @p_ticket_no, @p_name, @p_description, @p_due_date, @p_isapproved, @p_board_id, @p_label_id, @p_approved_by, @p_created_by, @p_updated_by, @assign_to, @assign_by, @p_move_to, @p_move_by, @p_ticket_label, @p_ticket_type, @p_project_id, @message, @status)", connection);
                var command = new NpgsqlCommand("CALL ticket.update_ticket_by_id(@p_ticket_id, @p_name, @p_description, @p_due_date, @p_isapproved, @p_board_id, @p_label_id, @p_updated_by, @assign_to, @assign_by, @p_move_to, @p_move_by, @p_ticket_type, @message, @status)", connection);


                command.Parameters.AddWithValue("p_ticket_id", dto.TicketId);
                //command.Parameters.AddWithValue("p_ticket_no", (object?)dto.TicketNo ?? DBNull.Value);
                command.Parameters.AddWithValue("p_name", (object?)dto.Name ?? DBNull.Value);
                command.Parameters.AddWithValue("p_description", (object?)dto.Description ?? DBNull.Value);
                command.Parameters.AddWithValue("p_due_date", (object?)dto.DueDate ?? DBNull.Value);
                command.Parameters.AddWithValue("p_isapproved", (object?)dto.IsApproved ?? DBNull.Value);
                command.Parameters.AddWithValue("p_board_id", (object?)dto.BoardId ?? DBNull.Value);
                command.Parameters.AddWithValue("p_label_id", (object?)dto.LabelId ?? DBNull.Value);
                //command.Parameters.AddWithValue("p_approved_by", (object?)dto.ApprovedBy ?? DBNull.Value);
                //command.Parameters.AddWithValue("p_created_by", (object?)dto.CreatedBy ?? DBNull.Value);
                command.Parameters.AddWithValue("p_updated_by", (object?)dto.UpdatedBy ?? DBNull.Value);
                //command.Parameters.AddWithValue("assign_to", (object?)dto.AssignTo ?? DBNull.Value);
                command.Parameters.Add(new NpgsqlParameter("assign_to", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                {
                    Value = dto.AssignTo ?? (object)DBNull.Value
                });
                command.Parameters.AddWithValue("assign_by", (object?)dto.AssignBy ?? DBNull.Value);
                //command.Parameters.AddWithValue("p_move_to", (object?)dto.MoveTo ?? DBNull.Value);
                command.Parameters.Add(new NpgsqlParameter("p_move_to", NpgsqlDbType.Array | NpgsqlDbType.Integer)
                {
                    Value = dto.MoveTo ?? (object)DBNull.Value
                });
                command.Parameters.AddWithValue("p_move_by", (object?)dto.MoveBy ?? DBNull.Value);
                //command.Parameters.AddWithValue("p_ticket_label", (object?)dto.TicketLabel ?? DBNull.Value);
                command.Parameters.AddWithValue("p_ticket_type", (object?)dto.TicketType ?? DBNull.Value);
                //command.Parameters.AddWithValue("p_project_id", (object?)dto.ProjectId ?? DBNull.Value);
              

              


                // OUTPUT params
                var messageParam = new NpgsqlParameter("message", DbType.String)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = ""
                };
                var statusParam = new NpgsqlParameter("status", DbType.String)
                {
                    Direction = ParameterDirection.InputOutput,
                    Value = ""
                };

                command.Parameters.Add(messageParam);
                command.Parameters.Add(statusParam);

                // Call
                await command.ExecuteNonQueryAsync();


                // Get the output values
                string message = messageParam.Value?.ToString() ?? "";
                string status = statusParam.Value?.ToString() ?? "";

                // Debug logging
                //Console.WriteLine($"SP execution result: {result}");
                Console.WriteLine($"Message param value: {message}");
                Console.WriteLine($"Status param value: {status}");

                return (dto.TicketId, message, status);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in UpdateTicketById: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return (dto.TicketId, $"Error: {ex.Message}", "Error");
            }
            finally
            {
                if (connection != null && connection.State != ConnectionState.Closed)
                {
                    try
                    {
                        await connection.CloseAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error closing connection");
                    }
                }
            }
        }


        public async Task<BaseResponse> ExecuteCreateTicketSp(CreateTicketDto dto)
        {
            var response = new BaseResponse();

            await using var conn = (NpgsqlConnection)Context.Database.GetDbConnection();
            await conn.OpenAsync();

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "ticket.create_ticket"; // updated stored procedure name
            cmd.CommandType = CommandType.StoredProcedure;

            // Add parameters
            //cmd.Parameters.Add(new NpgsqlParameter("p_project_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.ProjectId });
            cmd.Parameters.Add(new NpgsqlParameter("p_ticket_type", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dto.TicketType });
            cmd.Parameters.Add(new NpgsqlParameter("p_assign_to", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.AssignTo });
            cmd.Parameters.Add(new NpgsqlParameter("p_created_by", NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.CreatedBy });

            // New BOQ ID parameter
            cmd.Parameters.Add(new NpgsqlParameter("p_project_id", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = dto.ProjectId.HasValue ? dto.ProjectId.Value : (object)DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("p_po_id", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = dto.POId.HasValue ? dto.POId.Value : (object)DBNull.Value
            });

            cmd.Parameters.Add(new NpgsqlParameter("p_boq_id", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Value = dto.BoqId.HasValue ? dto.BoqId.Value : (object)DBNull.Value
            });

            var ticketIdParam = new NpgsqlParameter("p_ticket_id", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(ticketIdParam);

            var projectNameParam = new NpgsqlParameter("p_project_name", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(projectNameParam);


            var createdByParam = new NpgsqlParameter("o_created_by", NpgsqlTypes.NpgsqlDbType.Integer)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(createdByParam);

            var messageParam = new NpgsqlParameter("message", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };
            cmd.Parameters.Add(messageParam);

            var statusParam = new NpgsqlParameter("status", NpgsqlTypes.NpgsqlDbType.Varchar)
            {
                Direction = ParameterDirection.InputOutput,
                Value = DBNull.Value
            };
            cmd.Parameters.Add(statusParam);

            try
            {
                await cmd.ExecuteNonQueryAsync();

                response.Success = statusParam.Value?.ToString() == "true";
                response.Message = messageParam.Value?.ToString();
                response.Data = new
                {
                    TicketId = ticketIdParam.Value,
                    CreatedBy = createdByParam.Value,
                    ProjectName=projectNameParam.Value,
                };
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error";
            }

            return response;
        }


        //public async Task<BaseResponse> ExecuteCreateTicketSp(CreateTicketDto dto)
        //{
        //    var response = new BaseResponse();

        //    await using var conn = (NpgsqlConnection)Context.Database.GetDbConnection();
        //    await conn.OpenAsync();

        //    await using var cmd = conn.CreateCommand();
        //    cmd.CommandText = "ticket.create_ticket_sp2";
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    // Add parameters
        //    cmd.Parameters.Add(new NpgsqlParameter("p_project_id", NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.ProjectId });
        //    cmd.Parameters.Add(new NpgsqlParameter("p_ticket_type", NpgsqlTypes.NpgsqlDbType.Varchar) { Value = dto.TicketType });
        //    cmd.Parameters.Add(new NpgsqlParameter("p_assign_to", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.AssignTo });
        //    cmd.Parameters.Add(new NpgsqlParameter("p_created_by", NpgsqlTypes.NpgsqlDbType.Integer) { Value = dto.CreatedBy });

        //    var ticketIdParam = new NpgsqlParameter("p_ticket_id", NpgsqlTypes.NpgsqlDbType.Integer)
        //    {
        //        Direction = ParameterDirection.Output
        //    };
        //    cmd.Parameters.Add(ticketIdParam);

        //    var messageParam = new NpgsqlParameter("message", NpgsqlTypes.NpgsqlDbType.Varchar)
        //    {
        //        Direction = ParameterDirection.InputOutput,
        //        Value = DBNull.Value
        //    };
        //    cmd.Parameters.Add(messageParam);

        //    var statusParam = new NpgsqlParameter("status", NpgsqlTypes.NpgsqlDbType.Varchar)
        //    {
        //        Direction = ParameterDirection.InputOutput,
        //        Value = DBNull.Value
        //    };
        //    cmd.Parameters.Add(statusParam);

        //    try
        //    {
        //        await cmd.ExecuteNonQueryAsync();

        //        response.Success = statusParam.Value?.ToString() == "true";
        //        response.Message = messageParam.Value?.ToString();
        //        response.Data = new
        //        {
        //            TicketId = ticketIdParam.Value
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = $"Error: {ex.Message}";
        //    }

        //    return response;
        //} //    var response = new BaseResponse();
        //    try
        //    {
        //        await using var conn = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        //        await conn.OpenAsync();

        //        // Create the command with the correct parameter order
        //        await using var cmd = conn.CreateCommand();
        //        cmd.CommandText = "ticket.create_ticket_sp_with_ticket_name";
        //        cmd.CommandType = CommandType.StoredProcedure;

        //        // Input parameters
        //        cmd.Parameters.Add(new NpgsqlParameter("p_project_id", NpgsqlTypes.NpgsqlDbType.Integer)
        //        {
        //            Value = dto.ProjectId.HasValue ? (object)dto.ProjectId.Value : DBNull.Value
        //        });

        //        cmd.Parameters.Add(new NpgsqlParameter("p_ticket_type", NpgsqlTypes.NpgsqlDbType.Varchar)
        //        {
        //            Value = !string.IsNullOrEmpty(dto.TicketType) ? (object)dto.TicketType : DBNull.Value
        //        });

        //        // Using empty array when AssignTo is null or empty
        //        cmd.Parameters.Add(new NpgsqlParameter("p_assign_to", NpgsqlTypes.NpgsqlDbType.Array | NpgsqlTypes.NpgsqlDbType.Integer)
        //        {
        //            Value = dto.AssignTo?.Any() == true ? dto.AssignTo : new int[] { }  // Ensure empty array if null
        //        });

        //        cmd.Parameters.Add(new NpgsqlParameter("p_created_by", NpgsqlTypes.NpgsqlDbType.Integer)
        //        {
        //            Value = dto.CreatedBy
        //        });

        //        cmd.Parameters.Add(new NpgsqlParameter("p_ticket_name", NpgsqlTypes.NpgsqlDbType.Varchar)
        //        {
        //            Value = !string.IsNullOrEmpty(dto.TicketName) ? (object)dto.TicketName : DBNull.Value
        //        });

        //        // Output parameter for ticketId
        //        var ticketIdParam = new NpgsqlParameter
        //        {
        //            ParameterName = "p_ticket_id",
        //            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Integer,
        //            Direction = ParameterDirection.InputOutput,
        //            Value = DBNull.Value
        //        };
        //        cmd.Parameters.Add(ticketIdParam);

        //        // INOUT parameters for message and status
        //        var messageParam = new NpgsqlParameter
        //        {
        //            ParameterName = "message",
        //            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar,
        //            Direction = ParameterDirection.InputOutput,
        //            Value = DBNull.Value
        //        };
        //        cmd.Parameters.Add(messageParam);

        //        var statusParam = new NpgsqlParameter
        //        {
        //            ParameterName = "status",
        //            NpgsqlDbType = NpgsqlTypes.NpgsqlDbType.Varchar,
        //            Direction = ParameterDirection.InputOutput,
        //            Value = DBNull.Value
        //        };
        //        cmd.Parameters.Add(statusParam);

        //        // Execute the stored procedure
        //        await cmd.ExecuteNonQueryAsync();

        //        // Capture the output message and status from INOUT parameters
        //        var message = messageParam.Value is DBNull ? null : messageParam.Value?.ToString();
        //        var status = statusParam.Value is DBNull ? null : statusParam.Value?.ToString();

        //        // Capture ticketId after execution
        //        var ticketId = ticketIdParam.Value is DBNull ? null : ticketIdParam.Value;

        //        // Respond based on the status
        //        if (status?.ToLower() == "true")
        //        {
        //            response.Success = true;
        //            response.Message = message ?? "Ticket created successfully.";
        //            response.Data = new
        //            {
        //                TicketId = ticketId
        //            };
        //        }
        //        else
        //        {
        //            response.Success = false;
        //            response.Message = message ?? "Failed to create ticket.";
        //        }
        //    }
        //    catch (NpgsqlException ex)
        //    {
        //        response.Success = false;
        //        response.Message = $"PostgreSQL Error: {ex.Message}";
        //        // Consider logging the exception details here
        //    }
        //    catch (Exception ex)
        //    {
        //        response.Success = false;
        //        response.Message = $"Unexpected Error: {ex.Message}";
        //        // Consider logging the exception details here
        //    }
        //    return response;
        //}
        ////{
        //    try
        //    {
        //        using var connection = Context.Database.GetDbConnection();
        //        await connection.OpenAsync();

        //        var query = "SELECT * FROM ticket.get_ticket_by_ticket_id(@p_ticket_id);";

        //        var result = await connection.QueryFirstOrDefaultAsync<TicketData>(query, new { p_ticket_id = ticketId });

        //        return result;
        //    }

        //    catch (Exception ex)
        //    {
        //        throw new Exception($"An unexpected error occurred: {ex.Message}");
        //    }
        //}

        //public async Task<TicketData> GetTicketById(int ticketId)
        //{
        //    var connection = Context.Database.GetDbConnection();
        //    await connection.OpenAsync();

        //    using var command = connection.CreateCommand();
        //    command.CommandType = CommandType.Text;
        //    command.CommandText = "SELECT * FROM ticket.get_ticket_by_ticket_id1(@p_ticket_id)";

        //    var parameter = command.CreateParameter();
        //    parameter.ParameterName = "p_ticket_id";
        //    parameter.Value = ticketId;
        //    parameter.DbType = DbType.Int32;
        //    command.Parameters.Add(parameter);

        //    using var reader = await command.ExecuteReaderAsync();

        //    TicketData ticket = null;

        //    var jsonOptions = new JsonSerializerOptions
        //    {
        //        PropertyNameCaseInsensitive = true
        //    };

        //    while (await reader.ReadAsync())
        //    {
        //        ticket = new TicketData
        //        {
        //            ticket_id = reader.GetInt32(0),
        //            ticket_no = reader.GetString(1),
        //            name = reader.GetString(2),
        //            description = reader.IsDBNull(3) ? null : reader.GetString(3),
        //            create_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
        //            due_date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
        //            isapproved = reader.GetBoolean(6),
        //            updated_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
        //            created_at = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
        //            board_id = reader.IsDBNull(9) ? null : reader.GetInt32(9),
        //            label_id = reader.IsDBNull(10) ? null : reader.GetInt32(10),
        //            approved_by = reader.IsDBNull(11) ? null : reader.GetInt32(11),
        //            created_by = reader.IsDBNull(12) ? null : reader.GetInt32(12),
        //            updated_by = reader.IsDBNull(13) ? null : reader.GetInt32(13),
        //            assign_by = reader.IsDBNull(14) ? null : reader.GetInt32(14),
        //            move_to = reader.IsDBNull(15) ? null : reader.GetInt32(15),
        //            ticket_label = reader.IsDBNull(16) ? null : reader.GetString(16),
        //            ticket_type = reader.IsDBNull(17) ? null : reader.GetString(17),
        //            transaction_id = reader.IsDBNull(18) ? null : reader.GetInt32(18),
        //            ticket_owner_name = reader.IsDBNull(19) ? null : reader.GetString(19),
        //            role_name = reader.IsDBNull(20) ? null : reader.GetString(20),

        //            CommentsAndAttachments = reader.IsDBNull(21) || string.IsNullOrWhiteSpace(reader.GetString(21))
        //                ? new List<TicketCommentWithAttachmentDto>()
        //                : JsonSerializer.Deserialize<List<TicketCommentWithAttachmentDto>>(reader.GetString(21), jsonOptions),

        //            Approvals = reader.IsDBNull(22) || string.IsNullOrWhiteSpace(reader.GetString(22))
        //                ? new List<TicketApprovalDto>()
        //                : JsonSerializer.Deserialize<List<TicketApprovalDto>>(reader.GetString(22), jsonOptions),

        //            Rejected = reader.IsDBNull(23) || string.IsNullOrWhiteSpace(reader.GetString(23))
        //                ? new List<TicketRejectDto>()
        //                : JsonSerializer.Deserialize<List<TicketRejectDto>>(reader.GetString(23), jsonOptions)
        //        };
        //    }

        //    await connection.CloseAsync();
        //    return ticket;
        //}


        public async Task<TicketData> GetTicketById(int ticketId)
        {
            var connection = Context.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandType = CommandType.Text;
            command.CommandText = "SELECT * FROM ticket.get_ticket_by_ticket_id2(@p_ticket_id)";

            var parameter = command.CreateParameter();
            parameter.ParameterName = "p_ticket_id";
            parameter.Value = ticketId;
            parameter.DbType = DbType.Int32;
            command.Parameters.Add(parameter);

            using var reader = await command.ExecuteReaderAsync();

            TicketData ticket = null;

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            while (await reader.ReadAsync())
            {
                ticket = new TicketData
                {
                    ticket_id = reader.GetInt32(0),
                    ticket_no = reader.GetString(1),
                    name = reader.GetString(2),
                    description = reader.IsDBNull(3) ? null : reader.GetString(3),
                    create_date = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                    due_date = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
                    isapproved = reader.GetInt32(6),
                    updated_at = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                    created_at = reader.IsDBNull(8) ? null : reader.GetDateTime(8),
                    board_id = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                    label_id = reader.IsDBNull(10) ? null : reader.GetInt32(10),
                    approved_by = reader.IsDBNull(11) ? null : reader.GetInt32(11),
                    created_by = reader.IsDBNull(12) ? null : reader.GetInt32(12),
                    updated_by = reader.IsDBNull(13) ? null : reader.GetInt32(13),
                    assign_by = reader.IsDBNull(14) ? null : reader.GetInt32(14),
                    move_to = reader.IsDBNull(15) ? null : reader.GetInt32(15),
                    ticket_label = reader.IsDBNull(16) ? null : reader.GetString(16),
                    ticket_type = reader.IsDBNull(17) ? null : reader.GetString(17),
                   transaction_id= reader.IsDBNull(18) ? null : reader.GetInt32(18),
                    ticket_owner_name = reader.IsDBNull(19) ? null : reader.GetString(19),
                    role_name = reader.IsDBNull(20) ? null : reader.GetString(20),

                    CommentsAndAttachments = reader.IsDBNull(21) || string.IsNullOrWhiteSpace(reader.GetString(21))
                        ? new List<TicketCommentWithAttachmentDto>()
                        : JsonSerializer.Deserialize<List<TicketCommentWithAttachmentDto>>(reader.GetString(21), jsonOptions),

                    ApprovalsGrouped = reader.IsDBNull(22) || string.IsNullOrWhiteSpace(reader.GetString(22))
                        ? new Dictionary<string, List<TicketApprovalDto>>()
                        : JsonSerializer.Deserialize<Dictionary<string, List<TicketApprovalDto>>>(reader.GetString(22), jsonOptions),
                    VendorId = reader.IsDBNull(23) ? null : reader.GetInt32(23),
                    VendorName= reader.IsDBNull(24) ? null : reader.GetString(24)

                };
            }

            await connection.CloseAsync();
            return ticket;
        }

        //public async Task<IActionResult> AddCommentAndAttachmentAsync(TicketCommentAttachmentInputDto inputDto)
        //{
        //    using var transaction = await Context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        // Insert comment
        //        var comment = new TicketComment
        //        {
        //            TicketId = inputDto.TicketId,
        //            Comment = inputDto.Comment,
        //            CreatedBy = inputDto.CreatedBy,
        //            //CreatedDate = DateTime.UtcNow.Date
        //        };

        //        Context.TicketComments.Add(comment);
        //        await Context.SaveChangesAsync();

        //        // Handle file upload
        //        if (inputDto.File != null && inputDto.File.Length > 0)
        //        {
        //            var fileUrl = await SaveTicketAttachmentAsync(inputDto.File, $"ticket_{inputDto.TicketId}");

        //            var attachment = new Infrastructure.Entities.Attachment
        //            {
        //                TicketId = inputDto.TicketId,
        //                FileName = inputDto.File.FileName,
        //                FilePath = fileUrl,
        //                CreatedBy = inputDto.CreatedBy,
        //                //CreatedDate = DateTime.UtcNow.Date
        //            };

        //            Context.Attachments.Add(attachment);
        //            await Context.SaveChangesAsync();
        //        }

        //        await transaction.CommitAsync();

        //        return new OkObjectResult(new { message = "Comment and attachment saved successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return new ObjectResult(new { error = ex.Message }) { StatusCode = 500 };
        //    }
        //}

        public async Task<TicketCommentAttachmentResponseDto> AddCommentAndAttachmentAsync(TicketCommentAttachmentInputDto inputDto)
        {
            string? filePathUrl = null;
            string? originalFileName = null;

            if (inputDto.File != null && inputDto.File.Length > 0)
            {
                filePathUrl = await SaveTicketAttachmentAsync(inputDto.File, $"ticket_{inputDto.TicketId}");
                originalFileName = inputDto.File.FileName;
            }

            var conn = Context.Database.GetDbConnection();
            await using var command = conn.CreateCommand();

            command.CommandText = "CALL ticket.insert_ticket_comment_with_attachment(@p_ticket_id, @p_comment, @p_created_by,@p_created_by_type, @p_file_name, @p_file_path)";
            command.CommandType = CommandType.Text;

            command.Parameters.Add(new Npgsql.NpgsqlParameter("@p_ticket_id", inputDto.TicketId));
            command.Parameters.Add(new Npgsql.NpgsqlParameter("@p_comment", (object?)inputDto.Comment ?? DBNull.Value));
            command.Parameters.Add(new Npgsql.NpgsqlParameter("@p_created_by", inputDto.CreatedBy));
            command.Parameters.Add(new Npgsql.NpgsqlParameter("@p_created_by_type", (object?)inputDto.CreatedByType ?? DBNull.Value));
            command.Parameters.Add(new Npgsql.NpgsqlParameter("@p_file_name", (object?)originalFileName ?? DBNull.Value));
            command.Parameters.Add(new Npgsql.NpgsqlParameter("@p_file_path", (object?)filePathUrl ?? DBNull.Value));

            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync();

            await command.ExecuteNonQueryAsync();
            await conn.CloseAsync();

            return new TicketCommentAttachmentResponseDto
            {
                TicketId = inputDto.TicketId,
                FilePath = filePathUrl
            };
        }


        private async Task<string> SaveTicketAttachmentAsync(IFormFile file, string prefix)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "attachments");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileExtension = Path.GetExtension(file.FileName);
            var uniqueFileName = $"{prefix}_{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Replace baseUrl with your actual configured base URL or inject via config
            //var relativePath = Path.Combine("uploads", "report", uniqueFileName); // for storing in DB
            //return relativePath;// $"{baseUrl}/attachments/{uniqueFileName}";

            var relativePath = Path.Combine("attachments", uniqueFileName);
            return relativePath;
        }

        public async Task<BaseResponse> CreateCustomTicketAsync(CreateCustomTicketRequestDto dto)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var parameters = new DynamicParameters();

            parameters.Add("p_ticket_name", dto.TicketName, DbType.String, ParameterDirection.Input);
            parameters.Add("p_created_by", dto.CreatedBy, DbType.Int32, ParameterDirection.Input);
            parameters.Add("p_ticket_id", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("message", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 500);
            parameters.Add("status", dbType: DbType.String, direction: ParameterDirection.InputOutput, size: 50);

            await connection.ExecuteAsync("ticket.create_custom_ticket_sp", parameters, commandType: CommandType.StoredProcedure);

            var ticketId = parameters.Get<int>("p_ticket_id");
            var message = parameters.Get<string>("message");
            var status = parameters.Get<string>("status");

            return new BaseResponse
            {
                Success = status?.ToLower() == "true",
                Message = message,
                Data = new { TicketId = ticketId }
            };
        }
    }
}

