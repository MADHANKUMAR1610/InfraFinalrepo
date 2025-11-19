using Buildflow.Infrastructure.Constants;
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
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialRepository> _logger;
        private readonly IDailyStockRepository _dailyStockRepository;

        public MaterialRepository(
    BuildflowAppContext context,
    ILogger<MaterialRepository> logger,
    IDailyStockRepository dailyStockRepository)
        {
            _context = context;
            _logger = logger;
            _dailyStockRepository = dailyStockRepository;
        }

        // ---------------------------------------------------------
        // MAIN MATERIAL LIST
        // ---------------------------------------------------------
        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            var result = new List<MaterialDto>();
            int serial = 1;

            // 1️⃣ Collect all items:
            // - Hardcoded RequiredStock
            // - Items from BOQ
            // - Items from Inward/Outward
            var boqItems = await _context.BoqItems
                .Where(b => b.Boq!.ProjectId == projectId)
                .ToListAsync();

            var dbItems = await _context.StockInwards
                  .Where(x => x.ProjectId == projectId)
                  .Select(x => x.Itemname)
                  .Union(
                      _context.StockOutwards
                      .Where(x => x.ProjectId == projectId)
                      .Select(x => x.ItemName)
                  )
                  .Distinct()
                  .ToListAsync();

            var allItems = DailyStockRequirement.RequiredStock.Keys
                .Union(dbItems)
                .Union(boqItems.Select(b => b.ItemName))
                .Where(i => i != null)
                .Distinct()
                .ToList();

            foreach (var itemName in allItems)
            {
                if (string.IsNullOrEmpty(itemName))
                    continue;

                // --------------------------------------
                // 2️⃣ Get yesterday values from DailyStock
                // --------------------------------------
                var yesterdayStock = await _context.DailyStocks
                   .FirstOrDefaultAsync(x =>
                        x.ProjectId == projectId &&
                        x.ItemName == itemName &&
                        x.Date == yesterday);

                decimal yesterdayRemaining = yesterdayStock?.RemainingQty ?? 0;
                decimal yesterdayInstock = yesterdayStock?.InStock ?? 0;

                // --------------------------------------
                // 3️⃣ Hardcoded Required Qty for Today
                // --------------------------------------
                decimal todayHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(itemName)
                    ? DailyStockRequirement.RequiredStock[itemName]
                    : 0;

                // --------------------------------------
                // 4️⃣ BOQ Required Qty (Only approved BOQ)
                // --------------------------------------
                decimal boqRequiredToday = boqItems
                    .Where(b => b.ItemName == itemName)
                    .Sum(b => (decimal)(b.Quantity ?? 0));

                // --------------------------------------
                // 5️⃣ Total Inward Today
                // --------------------------------------
                decimal todayInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId &&
                                x.Itemname == itemName &&
                                x.DateReceived.HasValue &&
                                x.DateReceived.Value.ToUniversalTime().Date == today)
                    .SumAsync(x => (decimal?)x.QuantityReceived) ?? 0;

                // --------------------------------------
                // 6️⃣ Total Outward Today
                // --------------------------------------
                decimal todayOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId &&
                                x.ItemName == itemName &&
                                x.DateIssued.HasValue &&
                                x.DateIssued.Value.ToUniversalTime().Date == today)
                    .SumAsync(x => (decimal?)x.IssuedQuantity) ?? 0;

                // --------------------------------------
                // 7️⃣ Instock Calculation (carry-forward)
                // --------------------------------------
                decimal instock = yesterdayInstock + todayInward - todayOutward;
                if (instock < 0) instock = 0;

                // --------------------------------------
                // 8️⃣ Required Qty FINAL FORMULA
                // --------------------------------------
                decimal required =
                    (yesterdayRemaining + todayHardcoded + boqRequiredToday)
                    - todayOutward
                    - instock;

                if (required < 0)
                    required = 0;

                // --------------------------------------
                // 9️⃣ LEVEL Calculation (based ONLY on Instock)
                // --------------------------------------
                string level;
                if (instock == 0)
                    level = "Urgent";
                else if (instock <= required / 3)
                    level = "High";
                else if (instock <= required * 2 / 3)
                    level = "Medium";
                else
                    level = "Low";

                // --------------------------------------
                // 🔟 STATUS
                // --------------------------------------
                // 10️⃣ STATUS (get from Ticket.Isapproved)
                //-------------------------------------------------------
                // 1️⃣ Check if BOQ exists for this item
                //-------------------------------------------------------
                string status = "";
                bool boqExists = await _context.BoqItems
                    .AnyAsync(b =>
                        b.Boq!.ProjectId == projectId &&
                        EF.Functions.ILike(b.ItemName, $"%{itemName}%")
                    );

                //-------------------------------------------------------
                // 2️⃣ If RequiredQty = 0 → leave empty
                //-------------------------------------------------------
                if (required == 0)
                {
                    status = "";  // deactivated
                }
                else if (!boqExists)
                {
                    //---------------------------------------------------
                    // 3️⃣ Hardcoded-only, no BOQ request
                    //---------------------------------------------------
                    status = "";  // deactivated
                }
                else
                {
                    //---------------------------------------------------
                    // 4️⃣ BOQ Exists → Fetch approval status
                    //---------------------------------------------------
                    int? isApproved = await _context.Tickets
                        .Where(t =>
                            t.TicketType == "BOQ_APPROVAL" &&
                            t.BoqId != null &&
                            t.Boq!.ProjectId == projectId &&
                            t.Boq.BoqItems.Any(i =>
                                EF.Functions.ILike(i.ItemName, $"%{itemName}%")
                            )
                        )
                        .Select(t => t.Isapproved)
                        .FirstOrDefaultAsync();

                    //---------------------------------------------------
                    // 5️⃣ BOQ Exists → Evaluate
                    //---------------------------------------------------
                    if (isApproved == 1 || isApproved == 2)
                        status = "Approved";
                    else
                        status = "Pending";
                }


                // 1️⃣ Try get unit from StockInwards
                string unit = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId &&
                                x.Itemname == itemName &&
                                !string.IsNullOrEmpty(x.Unit))
                    .Select(x => x.Unit)
                    .FirstOrDefaultAsync();

                // 2️⃣ If not available, get from StockOutwards
                if (string.IsNullOrEmpty(unit))
                {
                    unit = await _context.StockOutwards
                        .Where(x => x.ProjectId == projectId &&
                                    x.ItemName == itemName &&
                                    !string.IsNullOrEmpty(x.Unit))
                        .Select(x => x.Unit)
                        .FirstOrDefaultAsync();
                }

                // 3️⃣ If still empty, get unit from BOQ items
                if (string.IsNullOrEmpty(unit))
                {
                    unit = await _context.BoqItems
                        .Where(b => b.Boq!.ProjectId == projectId &&
                                    b.ItemName == itemName &&
                                    !string.IsNullOrEmpty(b.Unit))
                        .Select(b => b.Unit)
                        .FirstOrDefaultAsync();
                }

                // 4️⃣ Fallback
                if (string.IsNullOrEmpty(unit))
                    unit = "Units";


                // --------------------------------------
                // 1️⃣1️⃣ Update today's DailyStock
                // --------------------------------------
                await _dailyStockRepository.UpdateDailyStockAsync(
                    projectId,
                    itemName,
                    todayOutward,
                    todayInward
                );

                // --------------------------------------
                // 1️⃣2️⃣ Add into result
                // --------------------------------------
                result.Add(new MaterialDto
                {
                    SNo = serial++,
                    MaterialList = itemName,
                    InStockQuantity = $"{instock} {unit}",     // ← HERE
                    RequiredQuantity = $"{required} {unit}",
                    Level = level,
                    RequestStatus = status
                });
            }

            return result;
        }
    }
}
