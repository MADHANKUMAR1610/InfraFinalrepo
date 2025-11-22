using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class MilestoneMasterRepository : IMilestoneMasterRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly ILogger<MilestoneMasterRepository> _logger;

        public MilestoneMasterRepository(
            BuildflowAppContext context,
            ILogger<MilestoneMasterRepository> logger
        )
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<MilestoneMasterDto>> GetAllAsync()
        {
            return await _context.Milestonemasters
                .Select(m => new MilestoneMasterDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code
                })
                .ToListAsync();
        }

        public async Task<MilestoneMasterDto> GetByIdAsync(int id)
        {
            return await _context.Milestonemasters
                .Where(m => m.Id == id)
                .Select(m => new MilestoneMasterDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> CreateAsync(MilestoneMasterDto model)
        {
            var entity = new Milestonemaster
            {
                Name = model.Name,
                Code = model.Code
            };

            _context.Milestonemasters.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(MilestoneMasterDto model)
        {
            var entity = await _context.Milestonemasters.FindAsync(model.Id);
            if (entity == null)
                return false;

            entity.Name = model.Name;
            entity.Code = model.Code;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.Milestonemasters.FindAsync(id);
            if (entity == null)
                return false;

            _context.Milestonemasters.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<StatusMasterDto>> GetProjectStatusAsync()
        {
            return await _context.Projectstatusmasters
                .Select(x => new StatusMasterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code
                }).ToListAsync();
        }

        public async Task<List<StatusMasterDto>> GetTaskStatusAsync()
        {
            return await _context.Taskstatusmasters
                .Select(x => new StatusMasterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code
                }).ToListAsync();
        }
        public async Task<bool> CreateProjectMilestoneAsync(ProjectMilestoneDto dto)
        {
            var entity = new ProjectMilestone
            {
                ProjectId = dto.ProjectId,
                MilestoneName = dto.MilestoneName,
                MilestoneDescription = dto.MilestoneDescription,
                MilestoneStartDate = dto.MilestoneStartDate,
                MilestoneEndDate = dto.MilestoneEndDate,
                MilestoneStatus = dto.MilestoneStatus,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow
            };

            _context.ProjectMilestones.Add(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> DeleteProjectMilestoneAsync(int milestoneId)
        {
            var entity = await _context.ProjectMilestones.FindAsync(milestoneId);
            if (entity == null) return false;

            _context.ProjectMilestones.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(int projectId)
        {
            return await _context.ProjectMilestones
                .Where(m => m.ProjectId == projectId)
                .OrderBy(m => m.MilestoneId)
                .Select(m => new ProjectMilestoneDto
                {
                    MilestoneId = m.MilestoneId,
                    ProjectId = m.ProjectId,
                    MilestoneName = m.MilestoneName,
                    MilestoneDescription = m.MilestoneDescription,
                    MilestoneStartDate = m.MilestoneStartDate,
                    MilestoneEndDate = m.MilestoneEndDate,
                    MilestoneStatus = m.MilestoneStatus,
                    Remarks = m.Remarks
                })
                .ToListAsync();
        }

        public async Task<ProjectMilestoneDto?> GetProjectMilestoneByIdAsync(int milestoneId)
        {
            var e = await _context.ProjectMilestones.FindAsync(milestoneId);
            if (e == null) return null;

            return new ProjectMilestoneDto
            {
                MilestoneId = e.MilestoneId,
                ProjectId = e.ProjectId,
                MilestoneName = e.MilestoneName,
                MilestoneDescription = e.MilestoneDescription,
                MilestoneStartDate = e.MilestoneStartDate,
                MilestoneEndDate = e.MilestoneEndDate,
                MilestoneStatus = e.MilestoneStatus,
                Remarks = e.Remarks
            };
        }
    }
}
