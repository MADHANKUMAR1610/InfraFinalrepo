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
    public class MaterialStatusRepository:IMaterialStatusRepository
    {

        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialStatusRepository> _logger;
        private readonly IConfiguration _config;
        private IConfiguration configuration;

        public MaterialStatusRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<MaterialStatusRepository> logger)
        {
            this.configuration = configuration;
            _context = context;
            _logger = logger;
        }

        public async Task<List<MaterialStatusDto>> GetMaterialStatusAsync(int projectId)
        {
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

            foreach (var reqItem in DailyStockRequirement.RequiredStock)
            {
                var inwardItem = inwardGroups.FirstOrDefault(x => x.ItemName == reqItem.Key);
                var outwardItem = outwardGroups.FirstOrDefault(x => x.ItemName == reqItem.Key);

                decimal inwardQty = inwardItem?.Quantity ?? 0;
                decimal outwardQty = outwardItem?.Quantity ?? 0;
                string unit = inwardItem?.Unit ?? outwardItem?.Unit ?? "";

                decimal inStock = inwardQty - outwardQty;
                decimal requiredQty = Math.Max(reqItem.Value - inStock, 0);

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
