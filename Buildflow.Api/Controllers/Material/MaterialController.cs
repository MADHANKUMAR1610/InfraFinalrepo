using Buildflow.Service.Service.Material;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Material
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {

        private readonly MaterialService _materialService;

        public MaterialController(MaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet("{projectId}")]
        public async Task<IActionResult> GetMaterial(int projectId)
        {
            var result = await _materialService.GetMaterialAsync(projectId);
            return Ok(result);
        }
    }
}

