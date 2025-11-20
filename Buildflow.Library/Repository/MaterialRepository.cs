using Buildflow.Infrastructure.Constants;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
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

        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            // Ensure base DailyStock rows exist
            await _dailyStockRepository.ResetDailyStockAsync(projectId);

            // Load BOQ Items
            var boqItems = await _context.BoqItems
                .Where(b => b.Boq!.ProjectId == projectId)
                .ToListAsync();

            // Load Yesterday Stock
            var yesterdayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == yesterday)
                .ToListAsync();

            // Load today’s inwards
            var inwardsToday = await _context.StockInwards
                .Where(s => s.ProjectId == projectId &&
                            s.DateReceived!.Value.ToUniversalTime().Date == today)
                .GroupBy(s => s.Itemname)
                .Select(g => new { Item = g.Key, Qty = g.Sum(x => (decimal?)x.QuantityReceived) ?? 0 })
                .ToListAsync();

            // Load today’s outwards
            var outwardsToday = await _context.StockOutwards
                .Where(s => s.ProjectId == projectId &&
                            s.DateIssued!.Value.ToUniversalTime().Date == today)
                .GroupBy(s => s.ItemName)
                .Select(g => new { Item = g.Key, Qty = g.Sum(x => (decimal?)x.IssuedQuantity) ?? 0 })
                .ToListAsync();

            // Load distinct DB items
            var dbItems = await _context.StockInwards
                .Where(x => x.ProjectId == projectId)
                .Select(x => x.Itemname)
                .Union(_context.StockOutwards
                    .Where(x => x.ProjectId == projectId)
                    .Select(x => x.ItemName))
                .Distinct()
                .ToListAsync();

            // Merge all item names
            var allItems = DailyStockRequirement.RequiredStock.Keys
                .Union(dbItems)
                .Union(boqItems.Select(b => b.ItemName))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct()
                .ToList();

            // Load approval tickets
            var tickets = await _context.Tickets
               .Where(t =>
                    t.TicketType == "BOQ_APPROVAL" &&
                    t.BoqId != null &&
                    t.Boq!.ProjectId == projectId)
               .Include(t => t.Boq!.BoqItems)
               .ToListAsync();

            // Unit resolution
            var unitsFromInwards = await _context.StockInwards
                .Where(x => x.ProjectId == projectId && x.Unit != null)
                .GroupBy(x => x.Itemname)
                .Select(g => new { Item = g.Key, Unit = g.First().Unit })
                .ToListAsync();

            var unitsFromOutwards = await _context.StockOutwards
                .Where(x => x.ProjectId == projectId && x.Unit != null)
                .GroupBy(x => x.ItemName)
                .Select(g => new { Item = g.Key, Unit = g.First().Unit })
                .ToListAsync();

            var unitsFromBoq = boqItems
                .Where(b => b.Unit != null)
                .GroupBy(b => b.ItemName)
                .Select(g => new { Item = g.Key, Unit = g.First().Unit })
                .ToList();

            // Build lookup dictionaries
            var inwardsTodayDict = inwardsToday.ToDictionary(x => x.Item!, x => x.Qty);
            var outwardsTodayDict = outwardsToday.ToDictionary(x => x.Item!, x => x.Qty);
            var yesterdayStockDict = yesterdayStocks.ToDictionary(x => x.ItemName!, x => x);

            var boqGroups = boqItems
                .GroupBy(b => b.ItemName)
                .ToDictionary(g => g.Key!, g => g.Sum(x => (decimal)(x.Quantity ?? 0)));

            var unitDict = new Dictionary<string, string>();
            foreach (var u in unitsFromInwards) unitDict[u.Item!] = u.Unit!;
            foreach (var u in unitsFromOutwards) if (!unitDict.ContainsKey(u.Item!)) unitDict[u.Item!] = u.Unit!;
            foreach (var u in unitsFromBoq) if (!unitDict.ContainsKey(u.Item!)) unitDict[u.Item!] = u.Unit!;

            // Approval dictionary
            var approvalMap = new Dictionary<string, int?>();
            foreach (var item in allItems)
            {
                var match = tickets.FirstOrDefault(t =>
                    t.Boq!.BoqItems.Any(b =>
                        b.ItemName != null &&
                        b.ItemName.ToLower().Contains(item.ToLower())
                    )
                );

                approvalMap[item] = match?.Isapproved;
            }

            // Today's stock
            var todayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == today)
                .ToListAsync();

            var todayStockDict = todayStocks.ToDictionary(d => d.ItemName!, d => d);

            var result = new List<MaterialDto>();
            int serial = 1;

            foreach (var item in allItems)
            {
                var key = item;

                // Yesterday
                yesterdayStockDict.TryGetValue(key, out var yStock);
                decimal yRem = yStock?.RemainingQty ?? 0;
                decimal yInstock = yStock?.InStock ?? 0;

                // Hardcoded
                bool isHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(key);
                decimal hardcodedReq = isHardcoded ? DailyStockRequirement.RequiredStock[key] : 0;

                // BOQ required
                decimal boqReq = boqGroups.ContainsKey(key) ? boqGroups[key] : 0;

                // Inward/outward
                inwardsTodayDict.TryGetValue(key, out var inward);
                outwardsTodayDict.TryGetValue(key, out var outward);

                // Instock
                decimal instock = yInstock + inward - outward;
                if (instock < 0) instock = 0;

                // Required
                decimal required = (yRem + hardcodedReq + boqReq) - outward - instock;
                if (required < 0) required = 0;

                // LEVEL
                string level = "";
                if (required > 0)
                {
                    if (instock == 0) level = "Urgent";
                    else if (instock <= required / 3) level = "High";
                    else if (instock <= required * 2 / 3) level = "Medium";
                    else level = "Low";
                }

                // STATUS
                approvalMap.TryGetValue(key, out var statusValue);
                string status = "";

                if (!isHardcoded)
                {
                    status = statusValue switch
                    {
                        1 => "Approved",
                        2 => "Pending",
                        0 => "Rejected",
                        _ => ""
                    };
                }

                // UNIT
                unitDict.TryGetValue(key, out var unit);
                if (unit == null) unit = "Units";

                // -------------------------------
                // DB SAVE RULE
                // -------------------------------
                bool canSave =
                    isHardcoded ||  // always save hardcoded items
                    statusValue == 1; // save only approved BOQ items

                if (todayStockDict.TryGetValue(key, out var tStock))
                {
                    if (canSave)
                    {
                        tStock.InStock = instock;
                        tStock.RemainingQty = required;
                    }
                }
                else
                {
                    if (canSave)
                    {
                        _context.DailyStocks.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = key,
                            InStock = instock,
                            RemainingQty = required,
                            DefaultQty = isHardcoded ? hardcodedReq : 0,
                            Date = today
                        });
                    }
                }

                // Add to UI result always
                result.Add(new MaterialDto
                {
                    SNo = serial++,
                    MaterialList = key,
                    InStockQuantity = $"{(int)Math.Round(instock)} {unit}",
                    RequiredQuantity = $"{(int)Math.Round(required)} {unit}",
                    Level = level,
                    RequestStatus = status
                });
            }

            await _context.SaveChangesAsync();
            return result;
        }
    }
}
