using Buildflow.Service.Service.Milestone;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        //[HttpPost("project/createMilestone")]
        //public async Task<IActionResult> Create(ProjectMilestoneDto dto)
        //{
        //    var ok = await _service.CreateProjectMilestoneAsync(dto);
        //    return Ok(new { success = ok });
        //}
        [HttpDelete("delete/{milestoneId}")]
        public async Task<IActionResult> DeleteMilestone(int milestoneId)
        {
            if (milestoneId <= 0)
                return BadRequest("Invalid milestoneId");

            var result = await _service.DeleteMilestoneAsync(milestoneId);

            if ((bool)!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        //[HttpGet("Get_milestone_By_projectId/{projectId}")]
        //public async Task<IActionResult> GetProjectMilestones(int projectId)
        //{
        //    if (projectId <= 0) return BadRequest("Invalid projectId");
        //    var data = await _service.GetProjectMilestonesAsync(projectId);
        //    return Ok(data);
        //}

        //[HttpGet("Get_milestone_By_MilestoneId{milestoneId}")]
        //public async Task<IActionResult> GetByMilestoneId(int milestoneId)
        //{
        //    var data = await _service.GetProjectMilestoneByIdAsync(milestoneId);
        //    return data == null ? NotFound() : Ok(data);
        //}
        [HttpPost("createTaskMilestone")]
        public async Task<IActionResult> CreateTaskMilestone([FromBody] List<ProjectTaskDto> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0)
                return BadRequest(new { success = false, message = "Task list cannot be empty." });

            var ok = await _service.CreateTaskListAsync(dtoList);

            return Ok(new
            {
                success = ok,
                message = ok ? "Tasks created successfully." : "Failed to create tasks."
            });
        }



        [HttpPut("updateTaskMilestone")]
        public async Task<IActionResult> UpdateTaskMilestone([FromBody] List<ProjectTaskDto> tasks)
        {
            var result = await _service.UpdateTasksAsync(tasks);

            if (!result)
                return BadRequest("Failed to update tasks.");

            return Ok("Tasks updated successfully.");
        }

        [HttpDelete("deleteTaskMilestone/{taskId}")]
        public async Task<IActionResult> DeleteTaskMilestone(int taskId)
        {
            var result = await _service.DeleteTaskAsync(taskId);
            if ((bool)result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("createSubTaskMilestone")]
        public async Task<IActionResult> CreateSubTaskMilestone([FromBody] List<ProjectSubTaskDto> dtoList)
        {
            if (dtoList == null || dtoList.Count == 0)
                return BadRequest(new { success = false, message = "Task list cannot be empty." });

            var ok = await _service.CreateSubTaskListAsync(dtoList);

            return Ok(new
            {
                success = ok,
                message = ok ? "Tasks created successfully." : "Failed to create tasks."
            });
        }



        [HttpPut("updateSubTaskMilestone")]
        public async Task<IActionResult> UpdateSubTaskMilestone([FromBody] List<ProjectSubTaskDto> tasks)
        {
            var result = await _service.UpdateSubTasksAsync(tasks);

            if (!result)
                return BadRequest("Failed to update tasks.");

            return Ok("Tasks updated successfully.");
        }

        [HttpDelete("deleteSubTaskMilestone/{taskId}")]
        public async Task<IActionResult> DeleteSubTaskMilestone(int subtaskId)
        {
            var result = await _service.DeleteSubTaskAsync(subtaskId);
            if ((bool)result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("getunit_master")]
        public async Task<IActionResult> GetAllUnitMaster()
        {
            var result = await _service.GetAllUnitMasterAsync();
            return Ok(result);
        }
        [HttpGet("project/{projectId}/milestone-summary")]
        public async Task<IActionResult> GetMilestoneSummary(int projectId)
        {
            if (projectId <= 0)
                return BadRequest("Invalid projectId");

            var data = await _service.GetMilestoneSummaryAsync(projectId);
            return Ok(data);
        }
    }
}
