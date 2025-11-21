using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class MaterialStockAlertRepository : IMaterialStockAlertRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MaterialStockAlertRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IMaterialRepository _materialRepository;

        public MaterialStockAlertRepository(
            IConfiguration configuration,
            BuildflowAppContext context,
            ILogger<MaterialStockAlertRepository> logger,
            IMaterialRepository materialRepository)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _materialRepository = materialRepository;
        }

        public async Task<List<MaterialDto>> GetMaterialStockAlertsAsync(int projectId)
        {
            try
            {
                //  Fetch pre-calculated material data from MaterialRepository
                var allMaterials = await _materialRepository.GetMaterialAsync(projectId);

                //  Filter only urgent materials
                var urgentMaterials = allMaterials
                    .Where(m => string.Equals(m.Level, "Urgent", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return urgentMaterials;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching urgent MaterialStockAlerts");
                throw;
            }
        }
    }
}
