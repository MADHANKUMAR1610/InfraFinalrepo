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
            //  Get EmpId from JWT token
            string empIdString = User.FindFirst("EmpId")?.Value;

            if (string.IsNullOrEmpty(empIdString))
                return Unauthorized("EmpId missing in token.");

            int employeeId = int.Parse(empIdString);

            //  Fetch approved projects assigned to this employee
            var projects = await _projectService.GetApprovedProjectsByEmployeeAsync(employeeId);

            if (!projects.Any())
                return BadRequest("No approved projects assigned to this employee.");

           
            int projectId = projects.First().ProjectId;

            //  Call existing material logic
           
            var result = await _materialService.TriggerRecalculationIfNeededAsync(projectId);


            return Ok(result);
        }
    }
}
