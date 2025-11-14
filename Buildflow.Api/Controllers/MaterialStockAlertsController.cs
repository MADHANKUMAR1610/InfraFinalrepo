using Buildflow.Service.Service.MaterialStockAlert;
using Buildflow.Service.Service.Project;         // <-- Add this
using Microsoft.AspNetCore.Authorization;        // <-- Add this
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;                    // <-- Add this

namespace Buildflow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // Require authentication
    public class MaterialStockAlertsController : ControllerBase
    {
        private readonly MaterialStockAlertService _service;
        private readonly ProjectService _projectService;

        public MaterialStockAlertsController(MaterialStockAlertService service, ProjectService projectService)
        {
            _service = service;
            _projectService = projectService;
        }

        // New endpoint - NO projectId in route
        [HttpGet("my-project/alerts")]
        public async Task<IActionResult> GetMaterialStockAlertsForLoggedInEngineer()
        {
            // 1️⃣ Extract EmpId from JWT token
            var empIdString = User.FindFirst("EmpId")?.Value;

            if (string.IsNullOrEmpty(empIdString))
                return Unauthorized("EmpId missing in token.");

            int employeeId = int.Parse(empIdString);

            // 2️⃣ Fetch approved projects assigned to this employee
            var projects = await _projectService.GetApprovedProjectsByEmployeeAsync(employeeId);

            if (!projects.Any())
                return BadRequest("No approved projects assigned to this employee.");

            // 3️⃣ Engineer has only one project → use the first
            int projectId = projects.First().ProjectId;

            // 4️⃣ Fetch material stock alerts using the projectId
            var result = await _service.GetMaterialStockAlertsAsync(projectId);

            return Ok(result);
        }
    }
}
