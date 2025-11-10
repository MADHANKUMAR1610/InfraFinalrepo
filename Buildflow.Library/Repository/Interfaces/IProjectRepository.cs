using Buildflow.Infrastructure.Entities;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface IProjectRepository : IGenericRepository<Project>
    {
        Task<List<BoqItemsDto>> GetBoqItemsByProjectIdAsync(int projectId);
        Task<BoqDetailsFullDto?> GetBoqDetailsAsync(int boqId);
        Task<IActionResult?> GetProjectTeamDetailsByProjectIdAsync(int projectId);
        Task DeleteProjectCascadeAsync(int projectId);
        Task<BoqDetailsFullDto?> GetBoqDetailsAsync(string code);
        Task<BaseResponse> UpsertProjectAsync(ProjectInput dto);
        Task UpsertProjectApprovalAsync(int approvedBy, int ticketId,string ApprovalType);
        Task<BaseResponse> UpsertProjectPermissionFinanceApproval(ProjectPermissionFinanceApprovalInputDto dto);
        Task<IActionResult> GetProjectDetailsByIdAsync(int projectId);
        Task<BaseResponse> UpsertBoqAsync(UpsertBoqRequestDto request);
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync(string? status);
        Task<IEnumerable<ProjectTypeDTO>> GetProjectTypesAsync();
        Task<IEnumerable<ProjectSectorDTO>> GetProjectSectorsAsync();
        Task<BaseResponse> UpsertProjectBudgetDetails(ProjectBudgetInputDto dto);
        Task<BaseResponse> UpsertProjectMilestoneDetails(ProjectMilestoneInputDto dto);

        Task<BaseResponse> UpsertProjectTeam(ProjectTeamInputDto dto);

        Task<(bool Success, string Message, object Data)> UpsertProjectTeamAsync(ProjectTeamUpsertDto dto);
       
    }
}
