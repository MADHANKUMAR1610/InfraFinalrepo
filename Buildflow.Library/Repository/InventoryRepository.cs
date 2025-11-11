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
        public InventoryRepository(IConfiguration configuration,BuildflowAppContext context, ILogger<GenericRepository<StockInward>> logger) : base(context, logger)
        {
            _logger = logger;       
            _configuration = configuration;
            _context = context;
        }



        public IDbConnection CreateConnection() =>
            new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        // ---------------------- CREATE STOCK INWARD ---------------------- //
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

                var vendorName = await _context.Vendors
                       .Where(e => e.VendorId == inward.VendorId)
                       .Select(e => e.VendorName)
                       .FirstOrDefaultAsync();

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
                    ReceivedById = inward.ReceivedbyId,
                    ReceivedByName = receivedByName,
                    Unit = inward.Unit,
                    Status = inward.Status,
                    Remarks = inward.Remarks,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Stock Inward entry: {Inner}", ex.InnerException?.Message ?? ex.Message);
                throw new ApplicationException($"Error while creating Stock Inward entry: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }


        // ---------------------- CREATE STOCK OUTWARD ---------------------- //
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
                    Remarks = dto.Remarks,
                    
                };

                await _context.StockOutwards.AddAsync(outward);
                await _context.SaveChangesAsync();

                
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
                _logger.LogError(ex, "Error creating Stock outward entry: {Inner}", ex.InnerException?.Message ?? ex.Message);
                throw new ApplicationException($"Error while creating Stock outward entry: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
        }


    }
}
    
