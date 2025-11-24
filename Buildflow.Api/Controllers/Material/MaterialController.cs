using Buildflow.Service.Service.Material;
using Buildflow.Service.Service.Project;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Buildflow.Api.Controllers.Material
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly MaterialService _materialService;
        private readonly ProjectService _projectService;

        public MaterialController(MaterialService materialService, ProjectService projectService)
        {
            _materialService = materialService;
            _projectService = projectService;
        }

        [HttpGet("my-project/materials")]
        public async Task<IActionResult> GetMaterialForLoggedInEngineer()
        {
            // 1️⃣ Get EmpId from JWT
            string empIdString = User.FindFirst("EmpId")?.Value;

            if (string.IsNullOrEmpty(empIdString))
                return Unauthorized("EmpId missing in token.");

            int employeeId = int.Parse(empIdString);

            // 2️⃣ Fetch approved projects for this employee
            var projects = await _projectService.GetApprovedProjectsByEmployeeAsync(employeeId);

            if (!projects.Any())
                return BadRequest("No approved projects assigned to this employee.");

            // 3️⃣ Use first assigned project (as per your rule)
            int projectId = projects.First().ProjectId;

            // 4️⃣ NEW LOGIC → Call MaterialService → GetMaterialAsync
            var result = await _materialService.GetMaterialAsync(projectId);

            return Ok(result);
        }
        [HttpGet("aqs/materials/{projectId}")]
        public async Task<IActionResult> GetAqsMaterials(int projectId)
        {
            var materials = await _materialService.GetMaterialAsync(projectId);
            return Ok(materials);
        }



        [HttpGet("material-names/{projectId}")]
        public async Task<IActionResult> GetMaterialNames(int projectId)
        {
            var data = await _materialService.GetMaterialNamesAsync(projectId);
            return Ok(data);
        }


    }
}
