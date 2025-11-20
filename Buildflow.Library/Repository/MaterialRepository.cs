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

            // Ensure base daily stock rows exist
            await _dailyStockRepository.ResetDailyStockAsync(projectId);

            // Load BOQ items
            var boqItems = await _context.BoqItems
                .Where(b => b.Boq!.ProjectId == projectId)
                .ToListAsync();

            // Load yesterday daily stocks
            var yesterdayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == yesterday)
                .ToListAsync();

            // Today's approved movements (used for instock)
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

            var inwardsTodayDict = inwardsToday.ToDictionary(x => x.Item!, x => x.Qty);
            var outwardsTodayDict = outwardsToday.ToDictionary(x => x.Item!, x => x.Qty);
            var yesterdayStockDict = yesterdayStocks.ToDictionary(x => x.ItemName!, x => x);

            // Distinct items from DB movements + hardcoded + BOQ items
            var dbItems = await _context.StockInwards
                .Where(x => x.ProjectId == projectId)
                .Select(x => x.Itemname)
                .Union(_context.StockOutwards
                    .Where(x => x.ProjectId == projectId)
                    .Select(x => x.ItemName))
                .Distinct()
                .ToListAsync();

            var allItems = DailyStockRequirement.RequiredStock.Keys
                .Union(dbItems)
                .Union(boqItems.Select(b => b.ItemName))
                .Where(i => !string.IsNullOrWhiteSpace(i))
                .Distinct()
                .ToList();

            // Load approval tickets for BOQs (to decide status). Choose latest ticket per BoqId.
            var tickets = await _context.Tickets
                .Where(t => t.TicketType == "BOQ_APPROVAL" && t.BoqId != null && t.Boq!.ProjectId == projectId)
                .ToListAsync();

            // Build latest ticket status per BoqId (pick highest TicketId as latest if CreatedDate not available)
            var latestTicketByBoq = tickets
                .GroupBy(t => t.BoqId)
                .ToDictionary(g => g.Key!, g => g.OrderByDescending(t => t.TicketId).FirstOrDefault());

            // For each BoqItem, determine its approval status via its BoqId
            // We'll compute sums per item name split by approval status:
            // approvedSumByItem, pendingSumByItem, rejectedSumByItem
            var approvedSumByItem = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var pendingSumByItem = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
            var rejectedSumByItem = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

            foreach (var b in boqItems)
            {
                if (string.IsNullOrWhiteSpace(b.ItemName)) continue;

                int? status = null;
                if (b.BoqId != null && latestTicketByBoq.ContainsKey(b.BoqId))
                    status = latestTicketByBoq[b.BoqId]?.Isapproved;

                var key = b.ItemName!.Trim();
                var q = (decimal)(b.Quantity ?? 0);

                if (status == 1)
                {
                    if (!approvedSumByItem.ContainsKey(key)) approvedSumByItem[key] = 0;
                    approvedSumByItem[key] += q;
                }
                else if (status == 2)
                {
                    if (!pendingSumByItem.ContainsKey(key)) pendingSumByItem[key] = 0;
                    pendingSumByItem[key] += q;
                }
                else if (status == 0)
                {
                    if (!rejectedSumByItem.ContainsKey(key)) rejectedSumByItem[key] = 0;
                    rejectedSumByItem[key] += q;
                }
                else
                {
                    // No ticket -> treat as Pending (show as pending row but do not save)
                    if (!pendingSumByItem.ContainsKey(key)) pendingSumByItem[key] = 0;
                    pendingSumByItem[key] += q;
                }
            }

            // Units resolution
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

            var unitDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var u in unitsFromInwards) unitDict[u.Item!] = u.Unit!;
            foreach (var u in unitsFromOutwards) if (!unitDict.ContainsKey(u.Item!)) unitDict[u.Item!] = u.Unit!;
            foreach (var u in unitsFromBoq) if (!unitDict.ContainsKey(u.Item!)) unitDict[u.Item!] = u.Unit!;

            // Today's DailyStock rows
            var todayStocks = await _context.DailyStocks
                .Where(d => d.ProjectId == projectId && d.Date == today)
                .ToListAsync();
            var todayStockDict = todayStocks.ToDictionary(d => d.ItemName!, d => d);

            // Helper: level computation
            string ComputeLevel(decimal req, decimal stk)
            {
                if (req <= 0) return "";
                if (stk <= 0) return "Urgent";
                if (stk <= req / 3) return "Medium";
                if (stk <= req * 2 / 3) return "Low";
                return "";
            }

            var result = new List<MaterialDto>();
            int serial = 1;

            foreach (var key in allItems)
            {
                var trimmedKey = key.Trim();

                bool isHardcoded = DailyStockRequirement.RequiredStock.ContainsKey(trimmedKey);
                decimal hardcodedReq = isHardcoded ? DailyStockRequirement.RequiredStock[trimmedKey] : 0;

                var unit = unitDict.ContainsKey(trimmedKey) ? unitDict[trimmedKey] : "Units";

                // sums by status for this item
                approvedSumByItem.TryGetValue(trimmedKey, out var approvedQty);
                pendingSumByItem.TryGetValue(trimmedKey, out var pendingQty);
                rejectedSumByItem.TryGetValue(trimmedKey, out var rejectedQty);

                // Yesterday values
                yesterdayStockDict.TryGetValue(trimmedKey, out var yStock);
                decimal yRem = yStock?.RemainingQty ?? 0;
                decimal yInstock = yStock?.InStock ?? 0;

                // totals of approved movements (for totalInward/totalOutward) across DB (not just today)
                // Note: we calculate totalInward and totalOutward up to now to satisfy exact instock formula
                decimal totalInward = await _context.StockInwards
                    .Where(x => x.ProjectId == projectId && x.Itemname == trimmedKey && x.Status == "Approved")
                    .SumAsync(x => (decimal?)x.QuantityReceived ?? 0);

                decimal totalOutward = await _context.StockOutwards
                    .Where(x => x.ProjectId == projectId && x.ItemName == trimmedKey && x.Status == "Approved")
                    .SumAsync(x => (decimal?)x.IssuedQuantity ?? 0);

                // instock = totalInward - totalOutward (clamped to 0)
                decimal instock = totalInward - totalOutward;
                if (instock < 0) instock = 0;

                // FINAL required calculation based on Option A:
                // required = (yesterdayRemaining + hardcoded + approvedBOQ) - totalOutward - instock
                decimal displayRequiredMain = (yRem + hardcodedReq + approvedQty) - totalOutward - instock;
                if (displayRequiredMain < 0) displayRequiredMain = 0;

                // ---------------------------
                // HARD-CODED ITEMS - Main Row
                // ---------------------------
                if (isHardcoded)
                {
                    // main row always present (hardcoded base - if approved BOQ added)
                    string levelMain = ComputeLevel(displayRequiredMain, instock);

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = trimmedKey,
                        InStockQuantity = $"{(int)Math.Round(instock)} {unit}",
                        RequiredQuantity = $"{(int)Math.Round(displayRequiredMain)} {unit}",
                        RequestStatus = approvedQty > 0 ? "Approved" : "",
                        Level = levelMain
                    });

                    // Add separate BOQ rows for pending and rejected (do not show approved as separate)
                    if (pendingQty > 0)
                    {
                        result.Add(new MaterialDto
                        {
                            SNo = serial++,
                            MaterialList = trimmedKey,
                            InStockQuantity = $"0 {unit}",
                            RequiredQuantity = $"{(int)Math.Round(pendingQty)} {unit}",
                            RequestStatus = "Pending",
                            Level = ""
                        });
                    }

                    if (rejectedQty > 0)
                    {
                        result.Add(new MaterialDto
                        {
                            SNo = serial++,
                            MaterialList = trimmedKey,
                            InStockQuantity = $"0 {unit}",
                            RequiredQuantity = $"{(int)Math.Round(rejectedQty)} {unit}",
                            RequestStatus = "Rejected",
                            Level = ""
                        });
                    }

                    // SAVE to DailyStock: only save merged (hardcoded + approved) when there is approved BOQ (or when hardcoded alone already expected to saved)
                    // Based on earlier logic: Save only when approved (we will update existing row when approved)
                    if (approvedQty > 0)
                    {
                        decimal saveRequired = hardcodedReq + approvedQty;
                        if (todayStockDict.TryGetValue(trimmedKey, out var stockRow))
                        {
                            stockRow.InStock = instock;
                            stockRow.RemainingQty = saveRequired;
                        }
                        else
                        {
                            _context.DailyStocks.Add(new DailyStock
                            {
                                ProjectId = projectId,
                                ItemName = trimmedKey,
                                InStock = instock,
                                RemainingQty = saveRequired,
                                DefaultQty = hardcodedReq,
                                Date = today
                            });
                        }
                    }
                    else
                    {
                        // If no approved BOQ, ensure hardcoded value exists in dailystock (depends on earlier ResetDailyStock behavior).
                        // Do NOT overwrite inStock/remaining if not intended; keep existing rows as they are.
                        if (!todayStockDict.ContainsKey(trimmedKey))
                        {
                            // create a row storing hardcoded default (so daily stock table has a row)
                            _context.DailyStocks.Add(new DailyStock
                            {
                                ProjectId = projectId,
                                ItemName = trimmedKey,
                                InStock = instock,
                                RemainingQty = hardcodedReq,
                                DefaultQty = hardcodedReq,
                                Date = today
                            });
                        }
                        else
                        {
                            // keep existing; do not add rejected/pending to RemainingQty
                        }
                    }

                    continue; // done with this item
                }

                // ---------------------------
                // NEW (NON-HARDCODED) ITEMS
                // ---------------------------
                // For new items, we do NOT render a main hardcoded row.
                // We render BOQ rows only (pending, rejected, approved)
                if (approvedQty > 0)
                {
                    // Approved BOQ row — this acts as main row for new items and gets level
                    string lvl = ComputeLevel(approvedQty, instock);

                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = trimmedKey,
                        InStockQuantity = $"{(int)Math.Round(instock)} {unit}",
                        RequiredQuantity = $"{(int)Math.Round(approvedQty)} {unit}",
                        RequestStatus = "Approved",
                        Level = lvl
                    });

                    // Save approved BOQ to DailyStock
                    if (todayStockDict.TryGetValue(trimmedKey, out var stockRow2))
                    {
                        stockRow2.InStock = instock;
                        stockRow2.RemainingQty = approvedQty;
                    }
                    else
                    {
                        _context.DailyStocks.Add(new DailyStock
                        {
                            ProjectId = projectId,
                            ItemName = trimmedKey,
                            InStock = instock,
                            RemainingQty = approvedQty,
                            DefaultQty = 0,
                            Date = today
                        });
                    }
                }

                if (pendingQty > 0)
                {
                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = trimmedKey,
                        InStockQuantity = $"{(int)Math.Round(instock)} {unit}",
                        RequiredQuantity = $"{(int)Math.Round(pendingQty)} {unit}",
                        RequestStatus = "Pending",
                        Level = "" // pending BOQ rows have no level
                    });
                }

                if (rejectedQty > 0)
                {
                    result.Add(new MaterialDto
                    {
                        SNo = serial++,
                        MaterialList = trimmedKey,
                        InStockQuantity = $"{(int)Math.Round(instock)} {unit}",
                        RequiredQuantity = $"{(int)Math.Round(rejectedQty)} {unit}",
                        RequestStatus = "Rejected",
                        Level = "" // rejected BOQ rows have no level
                    });
                }
            }

            await _context.SaveChangesAsync();
            return result;
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


    }
}
