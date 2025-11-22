using Buildflow.Service.Service.Milestone;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Milestone
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MilestoneMasterController : ControllerBase
    {
        private readonly MilestoneMasterService _service;

        public MilestoneMasterController(MilestoneMasterService service)
        {
            _service = service;
        }

        [HttpGet("allmilestonemaster")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("/get_milestoneMaster_by_id{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(id);
            return data == null ? NotFound() : Ok(data);
        }

        [HttpPost("create_milestoneMaster")]
        public async Task<IActionResult> Create(MilestoneMasterDto dto)
        {
            return Ok(await _service.CreateAsync(dto));
        }

        [HttpPut("update_milestoneMaster")]
        public async Task<IActionResult> Update(MilestoneMasterDto dto)
        {
            return Ok(await _service.UpdateAsync(dto));
        }

        [HttpDelete("delete_milestoneMaster_by_id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await _service.DeleteAsync(id));
        }
        [HttpGet("project_status_master")]
        public async Task<IActionResult> GetProjectStatus()
       => Ok(await _service.GetProjectStatusesAsync());

        [HttpGet("task_status_master")]
        public async Task<IActionResult> GetTaskStatus()
            => Ok(await _service.GetTaskStatusesAsync());
        [HttpPost("project/createMilestone")]
        public async Task<IActionResult> Create(ProjectMilestoneDto dto)
        {
            var ok = await _service.CreateProjectMilestoneAsync(dto);
            return Ok(new { success = ok });
        }
        [HttpDelete("project/DeleteMilestone/{milestoneId}")]
        public async Task<IActionResult> DeleteMilestone(int milestoneId)
        {
            var ok = await _service.DeleteProjectMilestoneAsync(milestoneId);
            return Ok(new { success = ok });
        }
        [HttpGet("Get_milestone_By_projectId/{projectId}")]
        public async Task<IActionResult> GetProjectMilestones(int projectId)
        {
            if (projectId <= 0) return BadRequest("Invalid projectId");
            var data = await _service.GetProjectMilestonesAsync(projectId);
            return Ok(data);
        }

        [HttpGet("Get_milestone_By_MilestoneId{milestoneId}")]
        public async Task<IActionResult> GetByMilestoneId(int milestoneId)
        {
            var data = await _service.GetProjectMilestoneByIdAsync(milestoneId);
            return data == null ? NotFound() : Ok(data);
        }
    }
}
