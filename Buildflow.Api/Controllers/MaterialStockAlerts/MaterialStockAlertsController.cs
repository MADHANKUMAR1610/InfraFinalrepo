using Buildflow.Service.Service.MaterialStockAlert;
using Buildflow.Service.Service.Project;      
using Microsoft.AspNetCore.Authorization;     
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;                  

namespace Buildflow.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class MaterialStockAlertsController : ControllerBase
    {
        private readonly MaterialStockAlertService _service;
        private readonly ProjectService _projectService;

        public MaterialStockAlertsController(MaterialStockAlertService service, ProjectService projectService)
        {
            _service = service;
            _projectService = projectService;
        }

        [HttpGet("my-project/alerts")]
        public async Task<IActionResult> GetMaterialStockAlertsForLoggedInEngineer()
        {
            //Extract EmpId from JWT token
            var empIdString = User.FindFirst("EmpId")?.Value;

            if (string.IsNullOrEmpty(empIdString))
                return Unauthorized("EmpId missing in token.");

            int employeeId = int.Parse(empIdString);

            //Fetch approved projects assigned to this employee
            var projects = await _projectService.GetApprovedProjectsByEmployeeAsync(employeeId);

            if (!projects.Any())
                return BadRequest("No approved projects assigned to this employee.");

           
            int projectId = projects.First().ProjectId;

            //  Fetch material stock alerts using the projectId
            var result = await _service.GetMaterialStockAlertsAsync(projectId);

            return Ok(result);
        }
    }
}
