using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Npgsql;
using Npgsql.Internal.Postgres;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Buildflow.Library.Repository
{
    public class ReportRepository : GenericRepository<Report>, IReportRepository
    {

        private readonly ILogger<GenericRepository<Infrastructure.Entities.Report>> _logger;

        private readonly IConfiguration _configuration;
        private readonly BuildflowAppContext _context;

        public ReportRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<Infrastructure.Entities.Report>> logger)
            : base(context, logger)
        {
            _logger = logger;
            _configuration = configuration;
            _context = context;
        }

        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<ReportDetails?> GetReportByIdAsync(int reportId)
        {
            try
            {
                var report = await _context.Reports
     //.Include(r => r.Project)
     .FirstOrDefaultAsync(r => r.ReportId == reportId);

                if (report == null)
                    return null;

                var reportType = await _context.ReportTypeMasters.FindAsync(report.ReportType);
                var project = await _context.Projects.FindAsync(report.ProjectId);


                var result = new ReportDetails
                {
                    ReportId = report.ReportId,
                    ReportCode = report.ReportCode,
                    ReportType = report.ReportType,
                    ReportTypeName = reportType?.ReportType,
                    ProjectId = report.ProjectId,
                    ProjectName = project?.ProjectName,
                    ReportDate = report.ReportDate,
                    ReportedBy = report.ReportedBy,
                    ReportData = JsonConvert.DeserializeObject<ReportDataDto>(report.ReportData ?? "{}")!,
                    CreatedAt = report.CreatedAt,
                    UpdatedAt = report.UpdatedAt
                };

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching report: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching the report.", ex);
            }
        }


        public async Task<List<ReportAttachment>> GetReportAttachmentByIdAsync(int reportId)
        {
            return await _context.ReportAttachments
                .AsNoTracking()
               .Where(r => r.ReportId == reportId)
               .ToListAsync();
        }

        public async Task<List<ReportDetails?>> GetReportsAsync()
        {
            //return await _context.Reports
            //    .AsNoTracking()
            //    .OrderByDescending(r => r.ReportDate)
            //    .ToListAsync();
            try
            {
                var reports = await _context.Reports.ToListAsync();

                var reportDetailsList = new List<ReportDetails>();

                foreach (var report in reports)
                {
                    var reportType = await _context.ReportTypeMasters.FindAsync(report.ReportType);
                    var project = await _context.Projects.FindAsync(report.ProjectId);

                    var reportDetails = new ReportDetails
                    {
                        ReportId = report.ReportId,
                        ReportCode = report.ReportCode,
                        ReportType = report.ReportType,
                        ReportTypeName = reportType?.ReportType,
                        ProjectId = report.ProjectId,
                        ProjectName = project?.ProjectName,
                        ReportDate = report.ReportDate,
                        ReportedBy = report.ReportedBy,
                        ReportData = JsonConvert.DeserializeObject<ReportDataDto>(report.ReportData ?? "{}")!,
                        CreatedAt = report.CreatedAt,
                        UpdatedAt = report.UpdatedAt
                    };

                    reportDetailsList.Add(reportDetails);
                }

                return reportDetailsList;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching report: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching the report.");
            }
        }

        public async Task UpsertReportAsync(Buildflow.Infrastructure.Entities.Report report)
        {
            try
            {
                if (report == null)
                    throw new ArgumentNullException(nameof(report));

                if (report.ReportId > 0)
                {
                    // UPDATE EXISTING REPORT
                    var existing = await _context.Reports.FirstOrDefaultAsync(r => r.ReportId == report.ReportId);

                    if (existing != null)
                    {
                        existing.ReportType = report.ReportType;
                        existing.ProjectId = report.ProjectId;
                        existing.ReportDate = report.ReportDate;
                        existing.ReportedBy = report.ReportedBy;
                        existing.ReportData = report.ReportData;
                        existing.UpdatedAt = DateTime.Now;

                        _context.Reports.Update(existing);
                    }
                }
                else
                {
                    report.CreatedAt = DateTime.Now;
                    report.UpdatedAt = DateTime.Now;

                    await _context.Reports.AddAsync(report);
                    await _context.SaveChangesAsync(); // Save to get ReportId for newly inserted report
                }

                // Insert Only NEW Assignees
                if (report.SendTo != null && report.SendTo.Any())
                {
                    // Get existing assignee IDs for this report
                    var existingAssigneeIds = await _context.ReportAssignees
                        .Where(ra => ra.ReportId == report.ReportId)
                        .Select(ra => ra.AssigneeId)
                        .ToListAsync();

                    // Filter out those already assigned
                    var newAssigneeIds = report.SendTo
                        .Except(existingAssigneeIds)
                        .Distinct()
                        .ToList();

                    if (newAssigneeIds.Any())
                    {
                        var newAssignees = newAssigneeIds
                            .Select(id => new ReportAssignee
                            {
                                ReportId = report.ReportId,
                                AssigneeId = id,
                                SendBy = report.SendBy,
                            }).ToList();

                        await _context.ReportAssignees.AddRangeAsync(newAssignees);
                    }
                }


                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException dbEx)
            {
                // Handle database-specific exceptions
                // Example: log and rethrow or convert to a custom exception
                throw new Exception("A database error occurred while saving the report.", dbEx);
            }
            catch (Exception ex)
            {
                // Handle all other exceptions
                throw new Exception("An error occurred while saving the report.", ex);
            }
        }

        //public Task<List<Report>> GetReportByReportType(int? type)
        //{
        //    throw new NotImplementedException();
        //}

        //public async Task<List<Report>> GetReportByReportType(int? typeId)
        //{
        //    try
        //    {

        //        var reports = await _context.Reports
        //            .Where(r => r.ReportType == typeId)
        //            .ToListAsync();



        //        return reports;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception here (e.g., using Serilog, NLog, etc.)
        //        Console.WriteLine($"Error fetching reports: {ex.Message}");
        //        throw new ApplicationException("An error occurred while fetching reports by report type.", ex);
        //    }
        //}

        public async Task<List<ReportDetails>> GetReportByReportType(int? typeId)
        {
            try
            {
                var reports = await _context.Reports.Where(r => r.ReportType == typeId).ToListAsync();

                var reportDetailsList = new List<ReportDetails>();

                foreach (var report in reports)
                {
                    var reportType = await _context.ReportTypeMasters.FindAsync(report.ReportType);
                    var project = await _context.Projects.FindAsync(report.ProjectId);

                    var reportDetails = new ReportDetails
                    {
                        ReportId = report.ReportId,
                        ReportCode = report.ReportCode,
                        ReportType = report.ReportType,
                        ReportTypeName = reportType?.ReportType,
                        ProjectId = report.ProjectId,
                        ProjectName = project?.ProjectName,
                        ReportDate = report.ReportDate,
                        ReportedBy = report.ReportedBy,
                        ReportData = JsonConvert.DeserializeObject<ReportDataDto>(report.ReportData ?? "{}")!,
                        CreatedAt = report.CreatedAt,
                        UpdatedAt = report.UpdatedAt
                    };

                    reportDetailsList.Add(reportDetails);
                }

                return reportDetailsList;
           
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reports: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching reports by report type.");
            }
        }


        public async Task<List<ReportDetails>> GetReportByEmpId(int? empId, int? typeId)
        {
            try
            {
                var reports = await _context.ReportAssignees
                    .Include(ra=>ra.Report)
                    .Where(ra => (ra.AssigneeId == empId||ra.SendBy==empId)&&
                    (typeId==null || ra.Report.ReportType==typeId))
                    .Select(ra => ra.Report)
                    .Distinct()
                    .ToListAsync();

                var reportDetailsList = new List<ReportDetails>();

                foreach (var report in reports)
                {
                    var reportType = await _context.ReportTypeMasters.FindAsync(report.ReportType);
                    var project = await _context.Projects.FindAsync(report.ProjectId);

                    var reportDetails = new ReportDetails
                    {
                        ReportId = report.ReportId,
                        ReportCode = report.ReportCode,
                        ReportType = report.ReportType,
                        ReportTypeName = reportType?.ReportType,
                        ProjectId = report.ProjectId,
                        ProjectName = project?.ProjectName,
                        ReportDate = report.ReportDate,
                        ReportedBy = report.ReportedBy,
                        ReportData = JsonConvert.DeserializeObject<ReportDataDto>(report.ReportData ?? "{}")!,
                        CreatedAt = report.CreatedAt,
                        UpdatedAt = report.UpdatedAt
                    };

                    reportDetailsList.Add(reportDetails);
                }

                return reportDetailsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching reports: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching reports for the employee.");
            }
        }


        public async Task<List<ReportTypeMaster>> GetReportTypes()
        {
            try
            {
                var reportTypes = await _context.ReportTypeMasters.ToListAsync();
                return reportTypes;
            }
            catch (Exception ex)
            {
                // Log the exception here
                Console.WriteLine($"Error fetching report types: {ex.Message}");
                throw new ApplicationException("An error occurred while fetching report types.");
            }
        }

    }
}
