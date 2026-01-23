using Buildflow.Infrastructure.Constants;
using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Buildflow.Utility.ENUM;


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

        // --------------------------
        // SAFE NORMALIZATION (EVERYWHERE)
        // --------------------------
        private string N(string s) => s?.Trim().ToLowerInvariant() ?? "";

        private string Display(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(raw.Trim().ToLowerInvariant());
        }

        public async Task<List<MaterialDto>> GetMaterialAsync(int projectId)
        {
            var today = DateTime.UtcNow.Date;
            var yesterday = today.AddDays(-1);

            await _dailyStockRepository.ResetDailyStockAsync(projectId);
             // keep behavior same: ensure base rows

            // --------------------------
            // LOAD DB DATA
            // --------------------------
            var boqItems = await _context.BoqItems
                .Where(b => b.Boq!.ProjectId == projectId)
                .ToListAsync();

            var yesterdayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == yesterday)
                .ToListAsync();

            var inwardsToday = await _context.StockInwards
                .Where(s => s.ProjectId == projectId && s.Status == "Approved" &&
                    s.DateReceived!.Value.ToUniversalTime().Date == today)
                .GroupBy(s => s.Itemname)
                .Select(g => new { Item = g.Key, Qty = g.Sum(x => (decimal?)x.QuantityReceived) ?? 0 })
                .ToListAsync();

            var outwardsToday = await _context.StockOutwards
                .Where(s => s.ProjectId == projectId && s.Status == "Approved" &&
                    s.DateIssued!.Value.ToUniversalTime().Date == today)
                .GroupBy(s => s.ItemName)
                .Select(g => new { Item = g.Key, Qty = g.Sum(x => (decimal?)x.IssuedQuantity) ?? 0 })
                .ToListAsync();


            // --------------------------
            // SAFE DICTIONARIES (FIX FOR DUPLICATE KEY)
            // --------------------------

            var inwardsTodayDict = inwardsToday
                .GroupBy(x => N(x.Item))
                .ToDictionary(g => g.Key, g => g.Sum(z => z.Qty), StringComparer.OrdinalIgnoreCase);

            var outwardsTodayDict = outwardsToday
                .GroupBy(x => N(x.Item))
                .ToDictionary(g => g.Key, g => g.Sum(z => z.Qty), StringComparer.OrdinalIgnoreCase);

            var yesterdayStockDict = yesterdayStocks
                .GroupBy(x => N(x.ItemName))
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First(), StringComparer.OrdinalIgnoreCase);


            // --------------------------
            // ALL ITEMS (HARD-CODED + DB + BOQ)
            // --------------------------

            var dbItems = await _context.StockInwards
                .Where(x => x.ProjectId == projectId)
                .Select(x => x.Itemname)
                .Union(_context.StockOutwards
                    .Where(x => x.ProjectId == projectId)
                    .Select(x => x.ItemName))
                .Distinct()
                .ToListAsync();

            var allItems = DailyStockRequirement.RequiredStock.Keys
                .Concat(dbItems)
                .Concat(boqItems.Select(b => b.ItemName))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Select(i => N(i))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();


            // --------------------------
            // TICKET STATUS
            // --------------------------
            var tickets = await _context.Tickets
                .Where(t => t.TicketType == "BOQ_APPROVAL" &&
                            t.BoqId != null &&
                            t.Boq!.ProjectId == projectId)
                .ToListAsync();

            var ticketLatest = tickets
                .GroupBy(t => t.BoqId)
                .ToDictionary(g => g.Key!, g => g.OrderByDescending(z => z.TicketId).First());


            var approved = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var pending = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var rejected = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);


            foreach (var b in boqItems)
            {
                string key = N(b.ItemName);
                decimal qty = (decimal)(b.Quantity ?? 0);

                int? status = null;
                if (b.BoqId != null && ticketLatest.ContainsKey(b.BoqId))
                    status = (int)ticketLatest[b.BoqId].ApprovalStatus;


                if (status == 1)
                {
                    if (!approved.ContainsKey(key)) approved[key] = 0;
                    approved[key] += qty;
                }
                else if (status == 2)
                {
                    if (!pending.ContainsKey(key)) pending[key] = 0;
                    pending[key] += qty;
                }
                else if (status == 0)
                {
                    if (!rejected.ContainsKey(key)) rejected[key] = 0;
                    rejected[key] += qty;
                }

            }


            // --------------------------
            // UNITS
            // --------------------------
            var unitDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            void AddUnit(string name, string? unit)
            {
                if (unit == null) return;
                string k = N(name);
                if (!unitDict.ContainsKey(k))
                    unitDict[k] = unit;
            }

            foreach (var u in await _context.StockInwards.Where(x => x.ProjectId == projectId).ToListAsync())
                AddUnit(u.Itemname!, u.Unit);

            foreach (var u in await _context.StockOutwards.Where(x => x.ProjectId == projectId).ToListAsync())
                AddUnit(u.ItemName!, u.Unit);

            foreach (var u in boqItems)
                AddUnit(u.ItemName!, u.Unit);


            var todayRows = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == today)
                .ToListAsync();
            var todayDict = todayRows
                .GroupBy(d => N(d.ItemName))
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
            // --------------------------
            // RESULT BUILDING
            // --------------------------
            var result = new List<MaterialDto>();
            int s = 1;

            // NOTE: The code below expects DailyStockRequirement.RequiredStock keys to be normalized
            // (i.e. lower-cased / trimmed). You confirmed you've updated the hardcoded dictionary accordingly.

            foreach (var key in allItems)
            {
                string name = Display(key);

                bool isHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(key);
                decimal requiredBase = isHardcoded ? DailyStockRequirement.RequiredStock[key] : 0;

                string unit = unitDict.ContainsKey(key) ? unitDict[key] : "Units";

                approved.TryGetValue(key, out var appQty);
                pending.TryGetValue(key, out var pendQty);
                rejected.TryGetValue(key, out var rejQty);

                yesterdayStockDict.TryGetValue(key, out var yRow);
                decimal yRem = yRow?.RemainingQty ?? 0;

                decimal totalIn = await _context.StockInwards
     .Where(x =>
         x.ProjectId == projectId &&
         x.Status == "Approved" &&
         x.Itemname != null &&
         x.Itemname.Trim().ToLower() == key)
     .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                decimal totalOut = await _context.StockOutwards
    .Where(x =>
        x.ProjectId == projectId &&
        x.Status == "Approved" &&
        x.ItemName != null &&
        x.ItemName.Trim().ToLower() == key)
    .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);



                decimal instock = totalOut;

                // Required = (yesterday + baseRequired + approvedBOQ) - instock
                decimal required = Math.Max(
     (yRem + requiredBase + appQty) - instock,
     0);



                // --------------------------
                // HARD-CODED MAIN ROW
                // --------------------------
                // --------------------------
                // HARD-CODED MAIN ROW (with merged approved BOQ)
                // --------------------------
                if (isHardcoded)
                {
                    // MERGE APPROVED QTY INTO MAIN ROW
                    decimal mergedRequired = requiredBase + appQty;

                    // Final Required = (Yesterday + mergedRequired) - instock
                    decimal finalRequired = (yRem + mergedRequired) - instock;
                    if (finalRequired < 0)
                        finalRequired = 0;

                    // MAIN HARD-CODED ROW
                    result.Add(new MaterialDto
                    {
                        SNo = s++,
                        MaterialList = name,
                        InStockQuantity = $"{(int)instock} {unit}",
                        RequiredQuantity = $"{(int)finalRequired} {unit}",
                        Level = ComputeLevel(finalRequired, instock),
                        RequestStatus = appQty > 0 ? "Approved" : ""

                    });

                    // SEPARATE PENDING ROW
                    if (pendQty > 0)
                    {
                        result.Add(new MaterialDto
                        {
                            SNo = s++,
                            MaterialList = name,
                            InStockQuantity = $"0 {unit}",
                            RequiredQuantity = $"{(int)pendQty} {unit}",
                            RequestStatus = "Pending"
                        });
                    }

                    // SEPARATE REJECTED ROW
                    if (rejQty > 0)
                    {
                        result.Add(new MaterialDto
                        {
                            SNo = s++,
                            MaterialList = name,
                            InStockQuantity = $"0 {unit}",
                            RequiredQuantity = $"{(int)rejQty} {unit}",
                            RequestStatus = "Rejected"
                        });
                    }

                    // SAVE MERGED VALUES
                    decimal saveRequired = finalRequired;

                    if (!todayDict.TryGetValue(key, out var tRow))
                    {
                        _context.DailyStocks.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = name,
                            InStock = instock,
                            RemainingQty = saveRequired,
                            DefaultQty = requiredBase,
                            Date = today
                        });
                    }
                    else
                    {
                        tRow.InStock = instock;
                        tRow.RemainingQty = saveRequired;
                    }

                    continue;
                }


                // --------------------------
                // NEW ITEMS (NO HARD-CODED ROW)
                // --------------------------

                if (appQty > 0)
                {
                    result.Add(new MaterialDto
                    {
                        SNo = s++,
                        MaterialList = name,
                        InStockQuantity = $"{(int)instock} {unit}",
                        RequiredQuantity = $"{(int)appQty} {unit}",
                        RequestStatus = "Approved",
                        Level = ComputeLevel(appQty, instock)
                    });

                    if (!todayDict.TryGetValue(key, out var tRow))
                    {
                        _context.DailyStocks.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = name,
                            InStock = instock,
                            RemainingQty = appQty,
                            DefaultQty = 0,
                            Date = today
                        });
                    }
                    else
                    {
                        tRow.InStock = instock;
                        tRow.RemainingQty = appQty;
                    }
                }

                if (pendQty > 0)
                {
                    result.Add(new MaterialDto
                    {
                        SNo = s++,
                        MaterialList = name,
                        InStockQuantity = $"{(int)instock} {unit}",
                        RequiredQuantity = $"{(int)pendQty} {unit}",
                        RequestStatus = "Pending"
                    });
                }

                if (rejQty > 0)
                {
                    result.Add(new MaterialDto
                    {
                        SNo = s++,
                        MaterialList = name,
                        InStockQuantity = $"{(int)instock} {unit}",
                        RequiredQuantity = $"{(int)rejQty} {unit}",
                        RequestStatus = "Rejected"
                    });
                }
            }

            await _context.SaveChangesAsync();
            return result;
        }
        private string ComputeLevel(decimal req, decimal stk)
        {
            if (req <= 0)
                return "";

            // percentage from 0–10 scale
            decimal percentage = (stk / req) * 10;

            if (percentage <= 2.5m) return "Urgent";
            if (percentage <= 5m) return "High";
            if (percentage <= 7.5m) return "Medium";
            if (percentage <= 10m) return "Low";

            return ""; // above 10 => very safe
        }

        // small wrapper to preserve previous behavior name (ResetDailyStockAsync call)
        private async Task _daily_stock_repository_reset_check(int projectId)
        {
            // Call the existing repository method
            await _dailyStockRepository.ResetDailyStockAsync(projectId);
        }

        public async Task<IEnumerable<object>> GetMaterialSummaryAsync(int projectId)
        {
            return await _context.DailyStocks
                .Where(m => m.ProjectId == projectId)
                .Select(m => new
                {
                    ItemName = m.ItemName,
                    Required = m.RemainingQty,
                    InStock = m.InStock
                })
                .ToListAsync();
        }

        public async Task<List<string>> GetAllMaterialNamesAsync(int projectId)
        {
            // 1️⃣ Hardcoded items
            var hardcoded = DailyStockRequirement.RequiredStock.Keys
                .Select(x => x.Trim())
                .ToList();

            // 2️⃣ Get BOQ items with APPROVED tickets only
            var approvedBoqItems = await (
                from boq in _context.BoqItems
                join ticket in _context.Tickets
                    on boq.BoqId equals ticket.BoqId
                where boq.Boq!.ProjectId == projectId
                      && ticket.TicketType == "BOQ_APPROVAL"
                      && ticket.ApprovalStatus == TicketApprovalStatus.Approved
                      // Approved only
                      && boq.ItemName != null
                select boq.ItemName
            )
            .Distinct()
            .ToListAsync();

            // 3️⃣ Merge hardcoded + approved BOQ
            var all = hardcoded
                .Union(approvedBoqItems)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            return all;
        }




    }
}
