using Buildflow.Service.Service.Material;
using Buildflow.Service.Service.MaterialStatus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.MaterialStatus
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialStatusController : ControllerBase
    {
        private readonly MaterialStatusService _materialStatusService;

        public MaterialStatusController(MaterialStatusService materialStatusService)
        {
            _materialStatusService = materialStatusService;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetMaterialStatus(int projectId)
        {
            var result = await _materialStatusService.GetMaterialStatusAsync(projectId);
            return Ok(result);
        }
    }
}
