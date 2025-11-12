using Buildflow.Service.Service.MaterialStockAlert;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialStockAlertsController : ControllerBase
    {

        private readonly MaterialStockAlertService _service;

        public MaterialStockAlertsController(MaterialStockAlertService service)
        {
            _service = service;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetMaterialStockAlerts(int projectId)
        {
            var result = await _service.GetMaterialStockAlertsAsync(projectId);
            return Ok(result);
        }
    }
}
