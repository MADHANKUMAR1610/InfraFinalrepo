using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.UOW;
using Buildflow.Service.Service.Master;
using Buildflow.Service.Service.Project;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Buildflow.Api.Controllers.Project
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {

        private readonly ProjectService _projectService;
        private readonly IConfiguration _config;
        private readonly BuildflowAppContext _context;





        public ProjectController(ProjectService projectService, IConfiguration config, BuildflowAppContext context)
        {
            _projectService = projectService;
            _config = config;
            _context = context;
            //_env = env;
            //_logger = logger;
        }
        string baseUrl = "  https://buildflowtestingapi.crestclimbers.com";


        [HttpGet("getAllProjects")]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return Ok(projects);
        }


        [HttpGet("get-projects-by-status")]
        public async Task<IActionResult> GetProjects([FromQuery] string? status)
        {
            try
            {
                var result = await _projectService.GetProjectsAsync(status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to fetch projects", error = ex.Message });
            }
        }

        [HttpGet("getProjectDetails")]
        public async Task<IActionResult> GetProjectDetailsById(int projectId)
        {
            try
            {
                var result = await _projectService.GetProjectDetailsById(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("upsert-project-approval")]
        public async Task<IActionResult> UpsertProjectApproval([FromBody] ProjectApprovalDto approvalDto)
        {
            // Call the service method to upsert the approval
            await _projectService.UpsertProjectApprovalAsync(approvalDto.ApprovedBy, approvalDto.TicketId, approvalDto.ApprovalType);

            // Return a success response
            return Ok(new { message = "Project approval upserted successfully" });
        }


        [HttpPost("upsertRisk")]
        //[AllowAnonymous]
        public async Task<IActionResult> SaveRisk([FromBody] RiskManagementBulkInput input)
        {
            try
            {
                if (input?.Risks == null || !input.Risks.Any())
                {
                    return BadRequest("No risks provided.");
                }

                var resultList = new List<object>();

                foreach (var riskInput in input.Risks)
                {
                    if (riskInput.RiskId == 0)
                    {
                        // Check for existing category for same project
                        bool categoryExists = await _context.ProjectRiskManagements
                            .AnyAsync(r => r.CategoryName.ToLower() == riskInput.CategoryName.ToLower() && r.ProjectId == riskInput.ProjectId);

                        if (categoryExists || riskInput.CategoryName==null)
                        {
                            continue; // Skip insertion if category exists for the same project
                        }

                        string imageUrl = null;
                        if (riskInput.UploadedFile != null && riskInput.UploadedFile.Length > 0)
                        {
                            imageUrl = await SaveCategoryFileAsync(riskInput.UploadedFile, Guid.NewGuid().ToString());
                        }

                        var risk = new ProjectRiskManagement
                        {
                            CategoryName = riskInput.CategoryName,
                            RiskStatus = riskInput.Status,
                            ProjectId = riskInput.ProjectId,
                            ImageUrl = imageUrl,
                            Remarks=riskInput.Remarks,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.ProjectRiskManagements.Add(risk);

                        resultList.Add(new { ProjectId = risk.ProjectId, ImageUrl = risk.ImageUrl });
                    }
                    else
                    {
                        var existingRisk = await _context.ProjectRiskManagements
                            .FirstOrDefaultAsync(r => r.RiskId == riskInput.RiskId);

                        if (existingRisk == null)
                            return NotFound($"Risk with ID {riskInput.RiskId} not found");

                        existingRisk.CategoryName = riskInput.CategoryName;
                        existingRisk.RiskStatus = riskInput.Status;
                        existingRisk.Remarks = riskInput.Remarks;
                        existingRisk.UpdatedAt = DateTime.UtcNow;

                        // Do NOT update ImageUrl even if a new file is uploaded
                        _context.ProjectRiskManagements.Update(existingRisk);

                        resultList.Add(new { ProjectId = existingRisk.ProjectId, ImageUrl = existingRisk.ImageUrl });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Risks processed successfully",
                    data = resultList
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }


        //[HttpPost("upsertRisk")]
        //public async Task<IActionResult> SaveRisk([FromBody] RiskManagementBulkInput input)
        //{
        //    try
        //    {
        //        if (input?.Risks == null || !input.Risks.Any())
        //        {
        //            return BadRequest("No risks provided.");
        //        }

        //        var resultList = new List<object>();

        //        foreach (var riskInput in input.Risks)
        //        {
        //            string imageUrl = null;


        //            if (riskInput.UploadedFile != null && riskInput.UploadedFile.Length > 0)
        //            {
        //                imageUrl = await SaveCategoryFileAsync(riskInput.UploadedFile, Guid.NewGuid().ToString());
        //            }

        //            if (riskInput.RiskId == 0)
        //            {
        //                var risk = new ProjectRiskManagement
        //                {
        //                    CategoryName = riskInput.CategoryName,
        //                    RiskStatus = riskInput.Status,
        //                    ProjectId = riskInput.ProjectId,
        //                    ImageUrl = imageUrl,
        //                    CreatedAt = DateTime.UtcNow
        //                };

        //                _context.ProjectRiskManagements.Add(risk);

        //                resultList.Add(new { ProjectId = risk.ProjectId, ImageUrl = risk.ImageUrl });
        //            }
        //            else
        //            {
        //                var existingRisk = await _context.ProjectRiskManagements
        //                    .FirstOrDefaultAsync(r => r.RiskId == riskInput.RiskId);

        //                if (existingRisk == null)
        //                    return NotFound($"Risk with ID {riskInput.RiskId} not found");

        //                existingRisk.CategoryName = riskInput.CategoryName;
        //                existingRisk.RiskStatus = riskInput.Status;
        //                existingRisk.ProjectId = riskInput.ProjectId;
        //                if (imageUrl != null)
        //                    existingRisk.ImageUrl = imageUrl;
        //                existingRisk.UpdatedAt = DateTime.UtcNow;

        //                _context.ProjectRiskManagements.Update(existingRisk);

        //                resultList.Add(new { ProjectId = existingRisk.ProjectId, ImageUrl = existingRisk.ImageUrl });
        //            }
        //        }

        //        // Save all changes in the database
        //        await _context.SaveChangesAsync();

        //        return Ok(new
        //        {
        //            message = "Risks processed successfully",
        //            data = resultList
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "An error occurred", error = ex.Message });
        //    }
        //}

        private async Task<string> SaveCategoryFileAsync(IFormFile file, string name)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var fileExtension = Path.GetExtension(file.FileName); 

            var fileName = $"{name}{fileExtension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }


        [HttpPost("upsertRisk-single-upload")]
        //[AllowAnonymous]

        public async Task<IActionResult> SaveSingleRisk([FromForm] RiskManagementInput input)
        {
            try
            {
                //if (input == null)
                //    return BadRequest("Invalid risk input.");

                string imageUrl = null;

                if (input.UploadedFile != null && input.UploadedFile.Length > 0)
                {
                    imageUrl = await SaveCategoryFileAsync(input.UploadedFile, Guid.NewGuid().ToString());
                }

                if (input.RiskId == 0)
                {
                    var newRisk = new ProjectRiskManagement
                    {
                        CategoryName = input.CategoryName,
                        RiskStatus = input.Status,
                        ProjectId = input.ProjectId,
                        ImageUrl = imageUrl,
                        Remarks=input.Remarks,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.ProjectRiskManagements.Add(newRisk);

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Risk created successfully",
                        data = new { ProjectId = newRisk.ProjectId, ImageUrl = newRisk.ImageUrl }
                    });
                }
                else
                {
                    var existingRisk = await _context.ProjectRiskManagements
                        .FirstOrDefaultAsync(r => r.RiskId == input.RiskId);

                    if (existingRisk == null)
                        return NotFound($"Risk with ID {input.RiskId} not found");

                    existingRisk.CategoryName = input.CategoryName;
                    existingRisk.RiskStatus = input.Status;
                    existingRisk.Remarks = input.Remarks;
                    existingRisk.ProjectId = input.ProjectId;
                    if (imageUrl != null)
                        existingRisk.ImageUrl = imageUrl;
                    existingRisk.UpdatedAt = DateTime.UtcNow;

                    _context.ProjectRiskManagements.Update(existingRisk);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        message = "Risk updated successfully",
                        data = new { ProjectId = existingRisk.ProjectId, ImageUrl = existingRisk.ImageUrl }
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }



        [HttpPost("upsertProject")]
        public async Task<IActionResult> AddOrUpdateProject([FromBody] ProjectInput projectInput)
        {
            try
            {
                var result = await _projectService.CreateProject(projectInput);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }




        [HttpPost("upsertProjectBudget")]
        public async Task<ActionResult> AddBudget(ProjectBudgetInputDto dto)
        {
            try
            {
                var result = await _projectService.InsertProjectBudgets(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }



        [HttpPost("upsertProjectTeam")]
        public async Task<IActionResult> UpsertProjectTeam([FromBody] ProjectTeamUpsertDto dto)
        {
            var (success, message, Data) = await _projectService.UpsertProjectTeam(dto);

            if (success)
                return Ok(new { success, message, data = Data });
            else
                return BadRequest(new { success, message });
        }


        [HttpGet("getProjectTypesAndProjectSectors")]
        public async Task<IActionResult> GetProjectTypeAndSector()
        {
            var response = await _projectService.GetProjectTypeAndSector();
            return Ok(response);
        }

        [HttpPost("upsertPermissionFinanceApproval")]
        public async Task<IActionResult> UpsertPermissionFinanceApproval([FromBody] ProjectPermissionFinanceApprovalInputDto dto)
        {
            try
            {
                var result = await _projectService.UpsertProjectPermissionFinanceApproval(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPost("upsertProjectMilestones")]
        public async Task<ActionResult> AddMilestones(ProjectMilestoneInputDto dto)
        {
            try
            {
                var result = await _projectService.InsertProjectMilestones(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpPost("upsertBoq")]

        public async Task<IActionResult> UpsertBoq([FromBody] UpsertBoqRequestDto request)
        {
            try
            {
                var result = await _projectService.UpsertBoqAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpGet("getBoqDetails")]


        public async Task<IActionResult> GetBoqDetails(int boqId)
        {
            try
            {
                var result = await _projectService.GetBoqDetailsAsync(boqId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        [HttpGet("getBoqDetailsBy-BOQCode")]


        public async Task<IActionResult> GetBoqDetailsBYBoqCode(string code)
        {
            try
            {
                var result = await _projectService.GetBoqDetailsAsync(code);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }


        [HttpGet("getNewBoqId")]
        public async Task<IActionResult> GetNewBoqId()
        {
            try
            {
                var boqId = await _context.Boqs.OrderByDescending(b => b.BoqId).FirstOrDefaultAsync();
                Random random = new Random();
                int nextBoqId = boqId != null ? boqId.BoqId + 1 : 0;

                return Ok("BOQ#" + nextBoqId);
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                return StatusCode(500, "An error occurred while generating BOQ ID.");
            }
        }

        [HttpGet("boq-items/{projectId}")]

        public async Task<IActionResult> GetBoqItems(int projectId)
        {
            var items = await _projectService.GetBoqItemsByProjectIdAsync(projectId);



            return Ok(items);
        }

        [HttpDelete("Delete-project/{projectid}")]
        public async Task<ActionResult> DeleteProjectCascadeAsync(int projectid)
        {
            await _projectService.DeleteProjectCascadeAsync(projectid);

            return Ok(new { message = "project deleted successfully." });
        }

        [HttpGet("get-ProjectTeamDetailsByProjectId/{projectId}")]
        public async Task<IActionResult> GetProjectTeamDetailsByProjectIdAsync(int projectId)
        {
            try
            {
                var result = await _projectService.GetProjectTeamDetailsByProjectIdAsync(projectId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }


        }
    }
}

 
