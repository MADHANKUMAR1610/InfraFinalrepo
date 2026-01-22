using Buildflow.Utility.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IMilestoneMasterRepository
    {
        Task<List<MilestoneMasterDto>> GetAllAsync();
        Task<MilestoneMasterDto> GetByIdAsync(int id);
        Task<bool> CreateAsync(MilestoneMasterDto model);
        Task<bool> UpdateAsync(MilestoneMasterDto model);
        Task<bool> DeleteAsync(int id);
        Task<List<StatusMasterDto>> GetProjectStatusAsync();
        Task<List<StatusMasterDto>> GetTaskStatusAsync();
        //    Task<bool> CreateProjectMilestoneAsync(ProjectMilestoneDto dto);
        Task<BaseResponse> DeleteMilestoneAsync(int milestoneId);

        //    Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(int projectId);
        //    Task<ProjectMilestoneDto?> GetProjectMilestoneByIdAsync(int milestoneId);
        Task<bool> CreateTaskListAsync(List<ProjectTaskDto> dtoList);
        Task<bool> UpdateTasksAsync(List<ProjectTaskDto> tasks);
        
        Task<BaseResponse> DeleteTaskAsync(int taskId);
        Task<bool> CreateSubTaskListAsync(List<ProjectSubTaskDto> dtoList);
        Task<bool> UpdateSubTasksAsync(List<ProjectSubTaskDto> tasks);

        Task<BaseResponse> DeleteSubTaskAsync(int taskId);

    }
}
