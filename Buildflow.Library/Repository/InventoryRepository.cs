using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class InventoryRepository : GenericRepository<StockInward>, IInventoryRepository
    {
        private readonly ILogger<GenericRepository<StockInward>> _logger;
        private readonly IConfiguration _configuration;
        private readonly BuildflowAppContext _context;
        private readonly IDailyStockRepository _dailyStockRepository;
        private readonly MaterialRepository _materialRepository;



        public InventoryRepository(IConfiguration configuration,BuildflowAppContext context, ILogger<GenericRepository<StockInward>> logger, IDailyStockRepository dailyStockRepository, IMaterialRepository materialRepository) : base(context, logger)
        {
            _logger = logger;       
            _configuration = configuration;
            _context = context;
            _dailyStockRepository = dailyStockRepository;
        }



        public IDbConnection CreateConnection() =>
            new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<StockInwardDto> CreateStockInwardAsync(StockInwardDto dto)
        {
            try
            {
                var inward = new StockInward
                {
                    ProjectId = dto.ProjectId,
                    Grn = dto.Grn,
                    Itemname = dto.Itemname,
                    VendorId = dto.VendorId,
                    QuantityReceived = dto.QuantityReceived,
                    Unit = dto.Unit,
                    DateReceived = dto.DateReceived?.ToUniversalTime() ?? DateTime.UtcNow,
                    ReceivedbyId = dto.ReceivedById,
                    Status = dto.Status ?? "Pending",
                    Remarks = dto.Remarks
                };

                await _context.StockInwards.AddAsync(inward);
                await _context.SaveChangesAsync();
                // 🔥 Trigger engineer material calculation for AQS flow
                await _materialRepository.GetMaterialAsync(inward.ProjectId);


                // ✔ Update Stock ONLY when Approved
                if (inward.Status == "Approved")
                {
                    await _dailyStockRepository.UpdateDailyStockAsync(
                        inward.ProjectId,
                        inward.Itemname,
                        outwardQty: 0,
                        inwardQty: inward.QuantityReceived ?? 0
                    );
                }


                // Fetch vendor name
                var vendorName = await _context.Vendors
                    .Where(v => v.VendorId == inward.VendorId)
                    .Select(v => v.VendorName)
                    .FirstOrDefaultAsync();

                // Fetch employee name
                var receivedByName = await _context.EmployeeDetails
                    .Where(e => e.EmpId == inward.ReceivedbyId)
                    .Select(e => e.FirstName + " " + e.LastName)
                    .FirstOrDefaultAsync();

                return new StockInwardDto
                {
                    StockinwardId = inward.StockinwardId,
                    ProjectId = inward.ProjectId,
                    Grn = inward.Grn,
                    Itemname = inward.Itemname,
                    VendorId = inward.VendorId,
                    VendorName = vendorName,
                    QuantityReceived = inward.QuantityReceived,
                    Unit = inward.Unit,
                    DateReceived = inward.DateReceived,
                    ReceivedById = inward.ReceivedbyId,
                    ReceivedByName = receivedByName,
                    Status = inward.Status,
                    Remarks = inward.Remarks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stock Inward");
                throw new ApplicationException("Error creating Stock Inward", ex);
            }
        }

        public async Task<StockOutwardDto> CreateStockOutwardAsync(StockOutwardDto dto)
        {
            try
            {
                var outward = new StockOutward
                {
                    ProjectId = dto.ProjectId,
                    IssueNo = dto.IssueNo,
                    ItemName = dto.ItemName,
                    RequestedById = dto.RequestedById,
                    IssuedQuantity = dto.IssuedQuantity,
                    Unit = dto.Unit,
                    IssuedToId = dto.IssuedToId,
                    DateIssued = dto.DateIssued?.ToUniversalTime() ?? DateTime.UtcNow,
                    Status = dto.Status ?? "Pending",
                    Remarks = dto.Remarks
                };

                await _context.StockOutwards.AddAsync(outward);
                await _context.SaveChangesAsync();
                // 🔥 Trigger engineer material calculation for AQS flow
                await _materialRepository.GetMaterialAsync(outward.ProjectId);


                if (outward.Status == "Approved")
                {
                    await _dailyStockRepository.UpdateDailyStockAsync(
                        outward.ProjectId,
                        outward.ItemName,
                        outwardQty: outward.IssuedQuantity ?? 0,
                        inwardQty: 0
                    );
                }

                var requestedByName = await _context.EmployeeDetails
                    .Where(e => e.EmpId == outward.RequestedById)
                    .Select(e => e.FirstName + " " + e.LastName)
                    .FirstOrDefaultAsync();

                var issuedToName = await _context.EmployeeDetails
                    .Where(e => e.EmpId == outward.IssuedToId)
                    .Select(e => e.FirstName + " " + e.LastName)
                    .FirstOrDefaultAsync();

                return new StockOutwardDto
                {
                    StockOutwardId = outward.StockOutwardId,
                    ProjectId = outward.ProjectId,
                    IssueNo = outward.IssueNo,
                    ItemName = outward.ItemName,
                    RequestedById = outward.RequestedById,
                    RequestedByName = requestedByName,
                    IssuedQuantity = outward.IssuedQuantity,
                    Unit = outward.Unit,
                    IssuedToId = outward.IssuedToId,
                    IssuedToName = issuedToName,
                    DateIssued = outward.DateIssued,
                    Status = outward.Status,
                    Remarks = outward.Remarks
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stock Outward");
                throw new ApplicationException("Error creating Stock Outward", ex);
            }
        }

        // GET STOCK INWARD BY PROJECT ID 
        public async Task<IEnumerable<StockInwardDto>> GetStockInwardsByProjectIdAsync(int projectId)
        {
            try
            {
                var inwards = await _context.StockInwards
                    .Where(i => i.ProjectId == projectId)
                    .ToListAsync();

                var result = new List<StockInwardDto>();

                foreach (var inward in inwards)
                {
                    var vendorName = await _context.Vendors
                        .Where(v => v.VendorId == inward.VendorId)
                        .Select(v => v.VendorName)
                        .FirstOrDefaultAsync();

                    var receivedByName = await _context.EmployeeDetails
                        .Where(e => e.EmpId == inward.ReceivedbyId)
                        .Select(e => e.FirstName + " " + e.LastName)
                        .FirstOrDefaultAsync();

                    result.Add(new StockInwardDto
                    {
                        StockinwardId = inward.StockinwardId,
                        ProjectId = inward.ProjectId,
                        Grn = inward.Grn,
                        Itemname = inward.Itemname,
                        VendorId = inward.VendorId,
                        VendorName = vendorName,
                        QuantityReceived = inward.QuantityReceived,
                        Unit = inward.Unit,
                        DateReceived = inward.DateReceived,
                        ReceivedById = inward.ReceivedbyId,
                        ReceivedByName = receivedByName,
                        Status = inward.Status,
                        Remarks = inward.Remarks
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Stock Inwards: {Inner}", ex.InnerException?.Message ?? ex.Message);
                throw new ApplicationException($"Error fetching Stock Inwards: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }


                            //  GET STOCK OUTWARD BY PROJECT ID 
        public async Task<IEnumerable<StockOutwardDto>> GetStockOutwardsByProjectIdAsync(int projectId)
        {
            try
            {
                var outwards = await _context.StockOutwards
                    .Where(o => o.ProjectId == projectId)
                    .ToListAsync();

                var result = new List<StockOutwardDto>();

                foreach (var outward in outwards)
                {
                    var requestedByName = await _context.EmployeeDetails
                        .Where(e => e.EmpId == outward.RequestedById)
                        .Select(e => e.FirstName + " " + e.LastName)
                        .FirstOrDefaultAsync();

                    var issuedToName = await _context.EmployeeDetails
                        .Where(e => e.EmpId == outward.IssuedToId)
                        .Select(e => e.FirstName + " " + e.LastName)
                        .FirstOrDefaultAsync();

                    result.Add(new StockOutwardDto
                    {
                        StockOutwardId = outward.StockOutwardId,
                        ProjectId = outward.ProjectId,
                        IssueNo = outward.IssueNo,
                        ItemName = outward.ItemName,
                        RequestedById = outward.RequestedById,
                        RequestedByName = requestedByName,
                        IssuedQuantity = outward.IssuedQuantity,
                        Unit = outward.Unit,
                        IssuedToId = outward.IssuedToId,
                        IssuedToName = issuedToName,
                        DateIssued = outward.DateIssued,
                        Status = outward.Status,
                        Remarks = outward.Remarks
                    });
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching Stock Outwards: {Inner}", ex.InnerException?.Message ?? ex.Message);
                throw new ApplicationException($"Error fetching Stock Outwards: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }
     
        public async Task<IEnumerable<object>> GetProjectTeamMembersAsync(int projectId)
        {
                        //  Get the project team record
            var projectTeam = await _context.ProjectTeams
                .FirstOrDefaultAsync(pt => pt.ProjectId == projectId);

            if (projectTeam == null)
                return Enumerable.Empty<object>();

                       // 2 Combine all employee ID lists
            var allEmployeeIds = new List<int>();

            if (projectTeam.PmId != null) allEmployeeIds.AddRange(projectTeam.PmId);
            if (projectTeam.ApmId != null) allEmployeeIds.AddRange(projectTeam.ApmId);
            if (projectTeam.LeadEnggId != null) allEmployeeIds.AddRange(projectTeam.LeadEnggId);
            if (projectTeam.SiteSupervisorId != null) allEmployeeIds.AddRange(projectTeam.SiteSupervisorId);
            if (projectTeam.QsId != null) allEmployeeIds.AddRange(projectTeam.QsId);
            if (projectTeam.AqsId != null) allEmployeeIds.AddRange(projectTeam.AqsId);
            if (projectTeam.SiteEnggId != null) allEmployeeIds.AddRange(projectTeam.SiteEnggId);
            if (projectTeam.EnggId != null) allEmployeeIds.AddRange(projectTeam.EnggId);
            if (projectTeam.DesignerId != null) allEmployeeIds.AddRange(projectTeam.DesignerId);
            if (projectTeam.VendorId != null) allEmployeeIds.AddRange(projectTeam.VendorId);
            if (projectTeam.SubcontractorId != null) allEmployeeIds.AddRange(projectTeam.SubcontractorId);

                           //  Remove duplicates
            allEmployeeIds = allEmployeeIds.Distinct().ToList();

                          //  Return ID + Name only
            var employees = await _context.EmployeeDetails
                .Where(e => allEmployeeIds.Contains(e.EmpId))
                .Select(e => new
                {
                    EmpId = e.EmpId,
                    FullName = e.FirstName +
                        (string.IsNullOrEmpty(e.LastName) ? "" : " " + e.LastName)
                })
                .ToListAsync();

            return employees;
        }


    }
}
 