using Buildflow.Infrastructure.Constants;
using Buildflow.Infrastructure.DatabaseContext;
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
    public class MaterialStatusRepository : IMaterialStatusRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialStatusRepository> _logger;
        private readonly IConfiguration _config;
        private readonly IConfiguration configuration;

        public MaterialStatusRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<MaterialStatusRepository> logger)
        {
            this.configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
            // 🧾 Get total inward (received) quantity per item
            var inwardGroups = await _context.StockInwards
                .Where(x => x.ProjectId == projectId)
                .GroupBy(x => new { x.Itemname, x.Unit })
                .Select(g => new
                {
                    ItemName = g.Key.Itemname,
                    Unit = g.Key.Unit ?? string.Empty,
                    Quantity = g.Sum(x => x.QuantityReceived) ?? 0
                })
                .ToListAsync();

            // 🧾 Get total outward (issued) quantity per item
            var outwardGroups = await _context.StockOutwards
                .Where(x => x.ProjectId == projectId)
                .GroupBy(x => new { x.ItemName, x.Unit })
                .Select(g => new
                {
                    ItemName = g.Key.ItemName,
                    Unit = g.Key.Unit ?? string.Empty,
                    Quantity = g.Sum(x => x.IssuedQuantity) ?? 0
                })
                .ToListAsync();

            var materials = new List<MaterialStatusDto>();

            // 🧮 Loop through each required stock item
            foreach (var reqItem in DailyStockRequirement.RequiredStock)
            {
                var inwardItem = inwardGroups.FirstOrDefault(x => x.ItemName == reqItem.Key);
                var outwardItem = outwardGroups.FirstOrDefault(x => x.ItemName == reqItem.Key);

                decimal inwardQty = inwardItem?.Quantity ?? 0;
                decimal outwardQty = outwardItem?.Quantity ?? 0;
                string unit = inwardItem?.Unit ?? outwardItem?.Unit ?? "";

                // 📦 Current in-stock quantity
                decimal inStock = inwardQty - outwardQty;

                // ✅ New Logic:
                // RequiredToBuy = TodayRequirement - (OutwardQty + InStock)
                decimal requiredQty = Math.Max(reqItem.Value - (outwardQty + inStock), 0);

                materials.Add(new MaterialStatusDto
                {
                    ItemName = reqItem.Key,
                    InStockDisplay = $"{inStock} {unit}",
                    RequiredDisplay = $"{requiredQty} {unit}"
                });
            }

            return materials;
        }
    }
}
