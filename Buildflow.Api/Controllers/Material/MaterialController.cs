using Buildflow.Service.Service.Material;
using Buildflow.Service.Service.Project;        
using Microsoft.AspNetCore.Authorization;        // <-- Add this
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;                    // <-- Add this

namespace Buildflow.Api.Controllers.Material
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]  // <-- Make sure endpoint requires authentication
    public class MaterialController : ControllerBase
    {
        private readonly MaterialService _materialService;
        private readonly ProjectService _projectService;

        public MaterialController(MaterialService materialService, ProjectService projectService)
        {
            _materialService = materialService;
            _projectService = projectService;
        }

        // NOW projectId is NOT passed in URL
        [HttpGet("my-project/materials")]
        public async Task<IActionResult> GetMaterialForLoggedInEngineer()
        {
            // 1️⃣ Get EmpId from JWT token
            string empIdString = User.FindFirst("EmpId")?.Value;

            if (string.IsNullOrEmpty(empIdString))
                return Unauthorized("EmpId missing in token.");

            int employeeId = int.Parse(empIdString);

            // 2️⃣ Fetch approved projects assigned to this employee
            var projects = await _projectService.GetApprovedProjectsByEmployeeAsync(employeeId);

            if (!projects.Any())
                return BadRequest("No approved projects assigned to this employee.");

            // 3️⃣ Select project (Engineer is assigned to only one project)
            int projectId = projects.First().ProjectId;

            // 4️⃣ Call existing material logic
           
            var result = await _materialService.TriggerRecalculationIfNeededAsync(projectId);


            return Ok(result);
        }
    }
}
