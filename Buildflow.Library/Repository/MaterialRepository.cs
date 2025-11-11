using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
   
       public class MaterialRepository : GenericRepository<BoqItem>, IMaterialRepository
        {
            private readonly BuildflowAppContext _context;
            private readonly ILogger<MaterialRepository> _logger;
            private readonly IConfiguration _config;

            public MaterialRepository(
                IConfiguration config,
                BuildflowAppContext context,
                ILogger<MaterialRepository> logger)
                : base(context, logger)
            {
                _config = config;
                _context = context;
                _logger = logger;
            }

            public async Task<IEnumerable<MaterialDto>> GetMaterialListAsync(int projectId)
            {
                try
                {
                    var query = from item in _context.BoqItems
                                join boq in _context.Boqs on item.BoqId equals boq.BoqId
                                join approval in _context.BoqApprovals on boq.BoqId equals approval.BoqId into boqApprovalGroup
                                from approval in boqApprovalGroup.DefaultIfEmpty()
                                join stock in _context.StockInwards on boq.ProjectId equals stock.ProjectId into stockGroup
                                from stock in stockGroup
                                    .Where(s => s.Itemname == item.ItemName)
                                    .DefaultIfEmpty()
                                where boq.ProjectId == projectId
                                select new MaterialDto
                                {
                                    MaterialList = item.ItemName,
                                    InStockQuantity = stock != null ? Convert.ToInt32(stock.QuantityReceived) : 0,
                                    RequiredQuantity = item.Quantity ?? 0,
                                    Level = (item.Quantity > (stock != null ? stock.QuantityReceived : 0))
                                            ? "High"
                                            : (item.Quantity == (stock != null ? stock.QuantityReceived : 0))
                                                ? "Medium"
                                                : "Low",
                                    RequestStatus = approval.ApprovalStatus ?? "Pending"
                                };

                    return await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error retrieving material list");
                    throw;
                }
            }
        }
    }

