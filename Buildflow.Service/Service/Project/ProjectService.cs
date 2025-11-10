using Buildflow.Library.UOW;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Buildflow.Infrastructure.Entities;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace Buildflow.Service.Service.Project
{
    public class ProjectService 
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProjectService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BoqDetailsFullDto> GetBoqDetailsAsync(int boqId)
        {
            return await _unitOfWork.Boq.GetBoqDetailsAsync(boqId);
        }

        public async Task<IActionResult?> GetProjectTeamDetailsByProjectIdAsync(int projectId)
        {
            var projectteams = await _unitOfWork.ProjectTeam.GetProjectTeamDetailsByProjectIdAsync(projectId);
            return projectteams;
        }

        public async Task DeleteProjectCascadeAsync(int projectid)
        {
            await _unitOfWork.Projects.DeleteProjectCascadeAsync(projectid);
        }
        public async Task<List<BoqItemsDto>> GetBoqItemsByProjectIdAsync(int projectId)
        {
            return await _unitOfWork.Boq.GetBoqItemsByProjectIdAsync(projectId);
        }
        public async Task<BoqDetailsFullDto> GetBoqDetailsAsync(string code)
        {
            return await _unitOfWork.Boq.GetBoqDetailsAsync(code);
        }
        public async Task<IEnumerable<ProjectDto>> GetProjectsAsync(string? status)
        {
            var projectDetails = await _unitOfWork.Projects.GetAllProjectsAsync(status);
            return projectDetails;
        }
        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projectDetails = await _unitOfWork.Projects.GetAllProjectsAsync();
            return projectDetails;
        }
        public async Task<IActionResult> GetProjectDetailsById(int projectId)
        {
            var projectDetails = await _unitOfWork.Projects.GetProjectDetailsByIdAsync(projectId);
            return projectDetails;
        }

        public async Task UpsertProjectApprovalAsync(int ApprovedBy, int TicketId,string ApprovalType)
        {
            // Call the repository to execute the stored procedure
            await _unitOfWork.Projects.UpsertProjectApprovalAsync(ApprovedBy, TicketId,ApprovalType);
        }
        public async Task<BaseResponse> CreateProject(ProjectInput dto)
        {

            try
            {
                var project = await _unitOfWork.Projects.UpsertProjectAsync(dto);
                await _unitOfWork.CompleteAsync(); // this line is throwing
                return project;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("EF Core Save Error: " + dbEx.InnerException?.Message, dbEx);
            }


        }

        




        public async Task<(bool Success, string Message, object Data)> UpsertProjectTeam(ProjectTeamUpsertDto dto)
        {
            return await _unitOfWork.ProjectTeam.UpsertProjectTeamAsync(dto);
        }

        public async Task<BaseResponse> UpsertProjectPermissionFinanceApproval(ProjectPermissionFinanceApprovalInputDto dto)
        {
            try
            {
                var result = await _unitOfWork.ProjectPermissionFinanceApprovals.UpsertProjectPermissionFinanceApproval(dto);
                await _unitOfWork.CompleteAsync(); // this line is throwing
                return result;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("EF Core Save Error: " + dbEx.InnerException?.Message, dbEx);
            }
        }

        public async Task<BaseResponse> InsertProjectBudgets(ProjectBudgetInputDto dto)
        {
            return await _unitOfWork.ProjectBudgets.UpsertProjectBudgetDetails(dto);
        }
      

        public async Task<ProjectTypeAndSector> GetProjectTypeAndSector()
        {
            var projectTypes = await _unitOfWork.ProjectTypes.GetProjectTypesAsync();
            var projectSectors = await _unitOfWork.ProjectSectors.GetProjectSectorsAsync();

            return new ProjectTypeAndSector
            {
                ProjectTypes = projectTypes.ToList(),
                ProjectSectors = projectSectors.ToList()
            };
        }

        public async Task<BaseResponse> InsertProjectMilestones(ProjectMilestoneInputDto dto)
        {
            return await _unitOfWork.ProjectMilestones.UpsertProjectMilestoneDetails(dto);
        }

        public async Task<BaseResponse> UpsertBoqAsync(UpsertBoqRequestDto request)
        {
            return await _unitOfWork.Boq.UpsertBoqAsync(request);
        }


    }
}
