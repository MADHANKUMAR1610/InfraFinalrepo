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

            await _daily_stock_repository_reset_check(projectId); // keep behavior same: ensure base rows

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
                    status = ticketLatest[b.BoqId].Isapproved;

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
                else
                {
                    if (!pending.ContainsKey(key)) pending[key] = 0;
                    pending[key] += qty;
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
                    .Where(x => x.ProjectId == projectId &&
                                x.Status == "Approved" &&
                                x.Itemname != null &&
                                x.Itemname.Trim().ToLower() == key)
                    .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                decimal totalOut = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId &&
                                x.Status == "Approved" &&
                                x.ItemName != null &&
                                x.ItemName.Trim().ToLower() == key)
                    .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                decimal instock = totalIn - totalOut;
                if (instock < 0) instock = 0;

                decimal required = (yRem + requiredBase + appQty) - totalOut - instock;
                if (required < 0) required = 0;

                // --------------------------
                // HARD-CODED MAIN ROW
                // --------------------------
                if (isHardcoded)
                {
                    result.Add(new MaterialDto
                    {
                        SNo = s++,
                        MaterialList = name,
                        InStockQuantity = $"{(int)instock} {unit}",
                        RequiredQuantity = $"{(int)required} {unit}",
                        Level = ComputeLevel(required, instock),
                        RequestStatus = appQty > 0 ? "Approved" : ""
                    });

                    if (pendQty > 0)
                        result.Add(new MaterialDto
                        {
                            SNo = s++,
                            MaterialList = name,
                            InStockQuantity = $"0 {unit}",
                            RequiredQuantity = $"{(int)pendQty} {unit}",
                            RequestStatus = "Pending"
                        });

                    if (rejQty > 0)
                        result.Add(new MaterialDto
                        {
                            SNo = s++,
                            MaterialList = name,
                            InStockQuantity = $"0 {unit}",
                            RequiredQuantity = $"{(int)rejQty} {unit}",
                            RequestStatus = "Rejected"
                        });

                    // SAVE ONLY APPROVED
                    if (appQty > 0)
                    {
                        decimal saveRequired = requiredBase + appQty;

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
            if (req <= 0) return "";
            if (stk <= 0) return "Urgent";
            if (stk <= req / 3) return "Medium";
            if (stk <= req * 2 / 3) return "Low";
            return "";
        }

        // small wrapper to preserve previous behavior name (ResetDailyStockAsync call)
        private async Task _daily_stock_repository_reset_check(int projectId)
        {
            // Call the existing repository method
            await _dailyStockRepository.ResetDailyStockAsync(projectId);
        }
    
public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            // get the same list you create in GetMaterialAsync()
            var materials = await GetMaterialAsync(projectId);

            // convert to MaterialStatusDto
            return materials.Select(m => new MaterialStatusDto
            {
                MaterialName = m.MaterialList,
                InStock = Convert.ToInt32(m.InStockQuantity.Split(' ')[0]),
                RequiredQty = Convert.ToInt32(m.RequiredQuantity.Split(' ')[0])
            })
            .ToList();
        }
        public async Task<List<string>> GetAllMaterialNamesAsync(int projectId)
        {
            // Hardcoded items
            var hardcoded = DailyStockRequirement.RequiredStock.Keys
                .Select(x => x.Trim())
                .ToList();

            // BOQ items (distinct)
            var boqItems = await _context.BoqItems
                .Where(b => b.Boq!.ProjectId == projectId)
                .Select(b => b.ItemName)
                .Distinct()
                .ToListAsync();

            // Merge + remove null and duplicates
            var all = hardcoded
                .Union(boqItems.Where(x => !string.IsNullOrWhiteSpace(x)))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();

            return all;
        }



    }
}
