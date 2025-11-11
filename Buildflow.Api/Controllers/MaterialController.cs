using Buildflow.Service.Service.Material;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly MaterialService _service;

        public MaterialController(MaterialService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get all material details for a specific project (joins BOQ, StockInward, and Approvals)
        /// </summary>
        /// <param name="projectId">Project ID</param>
        /// <returns>List of MaterialDto</returns>
        [HttpGet("get-material-list/{projectId}")]
        public async Task<ActionResult<IEnumerable<MaterialDto>>> GetMaterialList(int projectId)
        {
            if (projectId <= 0)
                return BadRequest("ProjectId is required.");

            var result = await _service.GetMaterialListAsync(projectId);

            if (result == null || !result.Any())
                return NotFound($"No materials found for ProjectId: {projectId}");

            return Ok(result);
        }
    }
}
