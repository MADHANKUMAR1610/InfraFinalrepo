using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.UOW;
using Buildflow.Service.Service.Inventory;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Inventory
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _service;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;
        private readonly BuildflowAppContext _context;

        public InventoryController(InventoryService service, IConfiguration config, IUnitOfWork unitOfWork, BuildflowAppContext context)
        {
            _service = service;
            _config = config;
            _unitOfWork = unitOfWork;
            _context = context;
        }
        [HttpPost("create-stock-inward")]
        public async Task<ActionResult<StockInwardDto>> CreateStockInward([FromBody] StockInwardDto input)
        {
            if (input.ProjectId <= 0)
                return BadRequest("ProjectId is required to create a stock inward record.");

            var result = await _service.CreateStockInwardAsync(input);
            return Ok(result);
        }

        [HttpPost("create-stock-outward")]
        public async Task<ActionResult<StockOutwardDto>> CreateStockOutward([FromBody] StockOutwardDto input)

        {
            if (input.ProjectId <= 0)
                return BadRequest("ProjectId is required to create a stock outward record.");
            var result = await _service.CreateStockOutwardAsync(input);
            return Ok(result);
        }
        [HttpGet("get-stock-inwards/{projectId}")]
        public async Task<ActionResult<IEnumerable<StockInwardDto>>> GetStockInwardsByProjectId(int projectId)
        {
            if (projectId <= 0)
                return BadRequest("Invalid ProjectId.");

            var result = await _service.GetStockInwardsByProjectIdAsync(projectId);
            return Ok(result);
        }

        [HttpGet("get-stock-outwards/{projectId}")]
        public async Task<ActionResult<IEnumerable<StockOutwardDto>>> GetStockOutwardsByProjectId(int projectId)
        {
            if (projectId <= 0)
                return BadRequest("Invalid ProjectId.");

            var result = await _service.GetStockOutwardsByProjectIdAsync(projectId);
            return Ok(result);
        }
        [HttpGet("project-team/{projectId}")]
        public async Task<IActionResult> GetProjectTeamMembers(int projectId)
        {
            var members = await _service.GetProjectTeamMembersAsync(projectId);
            return Ok(members);
        }


    }
}
