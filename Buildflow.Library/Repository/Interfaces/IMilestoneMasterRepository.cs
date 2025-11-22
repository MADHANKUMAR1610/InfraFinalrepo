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
        Task<bool> CreateProjectMilestoneAsync(ProjectMilestoneDto dto);
        Task<bool> DeleteProjectMilestoneAsync(int milestoneId);
        Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(int projectId);
        Task<ProjectMilestoneDto?> GetProjectMilestoneByIdAsync(int milestoneId);
    }
}
