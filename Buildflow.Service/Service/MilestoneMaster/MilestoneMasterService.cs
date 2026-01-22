using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Milestone
{
    public class MilestoneMasterService
    {
        private readonly IMilestoneMasterRepository _repo;

        public MilestoneMasterService(IMilestoneMasterRepository repo)
        {
            _repo = repo;
        }

        public Task<List<MilestoneMasterDto>> GetAllAsync()
            => _repo.GetAllAsync();

        public Task<MilestoneMasterDto> GetByIdAsync(int id)
            => _repo.GetByIdAsync(id);

        public Task<bool> CreateAsync(MilestoneMasterDto model)
            => _repo.CreateAsync(model);

        public Task<bool> UpdateAsync(MilestoneMasterDto model)
            => _repo.UpdateAsync(model);

        public Task<bool> DeleteAsync(int id)
            => _repo.DeleteAsync(id);
        public Task<List<StatusMasterDto>> GetProjectStatusesAsync()
       => _repo.GetProjectStatusAsync();

        public Task<List<StatusMasterDto>> GetTaskStatusesAsync()
            => _repo.GetTaskStatusAsync();
        //  public Task<bool> CreateProjectMilestoneAsync(ProjectMilestoneDto dto)
        //    => _repo.CreateProjectMilestoneAsync(dto);
        //  public Task<bool> DeleteProjectMilestoneAsync(int milestoneId)
        //=> _repo.DeleteProjectMilestoneAsync(milestoneId);
        //  public Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(int projectId)
        //    => _repo.GetProjectMilestonesAsync(projectId);

        //  public Task<ProjectMilestoneDto?> GetProjectMilestoneByIdAsync(int milestoneId)
        //      => _repo.GetProjectMilestoneByIdAsync(milestoneId);
        public async Task<bool> CreateTaskListAsync(List<ProjectTaskDto> dtoList)
        {
            return await _repo.CreateTaskListAsync(dtoList);
        }



        public async Task<bool> UpdateTasksAsync(List<ProjectTaskDto> tasks)
        {
            return await _repo.UpdateTasksAsync(tasks);
        }
        public Task<BaseResponse> DeleteTaskAsync(int taskId)
            => _repo.DeleteTaskAsync(taskId);

        public Task<BaseResponse> DeleteMilestoneAsync(int milestoneId)
        {
            return _repo.DeleteMilestoneAsync(milestoneId);
        }

        public async Task<bool> CreateSubTaskListAsync(List<ProjectSubTaskDto> dtoList)
        {
            return await _repo.CreateSubTaskListAsync(dtoList);
        }



        public async Task<bool> UpdateSubTasksAsync(List<ProjectSubTaskDto> tasks)
        {
            return await _repo.UpdateSubTasksAsync(tasks);
        }
        public Task<BaseResponse> DeleteSubTaskAsync(int subtaskId)
            => _repo.DeleteSubTaskAsync(subtaskId);

    }
}
