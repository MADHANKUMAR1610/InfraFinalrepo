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
        private static DateOnly? ToDateOnly(DateTime? dt)
    => dt.HasValue ? DateOnly.FromDateTime(dt.Value) : (DateOnly?)null;

        private static DateTime? ToDateTime(DateOnly? d)
            => d.HasValue ? d.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null;

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
        // these all are for milestonemaster CRUD operations
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
        //public async Task<bool> CreateProjectMilestoneAsync(ProjectMilestoneDto dto)
        //{
        //    var entity = new ProjectMilestone
        //    {
        //        ProjectId = dto.ProjectId,
        //        MilestoneName = dto.MilestoneName,
        //        MilestoneDescription = dto.MilestoneDescription,
        //        MilestoneStartDate = dto.MilestoneStartDate,
        //        MilestoneEndDate = dto.MilestoneEndDate,
        //        MilestoneStatus = dto.MilestoneStatus,
        //        Remarks = dto.Remarks,
        //        CreatedAt = DateTime.UtcNow
        //    };

        //    _context.ProjectMilestones.Add(entity);
        //    return await _context.SaveChangesAsync() > 0;
        //}
        //public async Task<bool> DeleteProjectMilestoneAsync(int milestoneId)
        //{
        //    var entity = await _context.ProjectMilestones.FindAsync(milestoneId);
        //    if (entity == null) return false;

        //    _context.ProjectMilestones.Remove(entity);
        //    return await _context.SaveChangesAsync() > 0;
        //}
        //public async Task<List<ProjectMilestoneDto>> GetProjectMilestonesAsync(int projectId)
        //{
        //    return await _context.ProjectMilestones
        //        .Where(m => m.ProjectId == projectId)
        //        .OrderBy(m => m.MilestoneId)
        //        .Select(m => new ProjectMilestoneDto
        //        {
        //            MilestoneId = m.MilestoneId,
        //            ProjectId = m.ProjectId,
        //            MilestoneName = m.MilestoneName,
        //            MilestoneDescription = m.MilestoneDescription,
        //            MilestoneStartDate = m.MilestoneStartDate,
        //            MilestoneEndDate = m.MilestoneEndDate,
        //            MilestoneStatus = m.MilestoneStatus,
        //            Remarks = m.Remarks
        //        })
        //        .ToListAsync();
        //}

        //public async Task<ProjectMilestoneDto?> GetProjectMilestoneByIdAsync(int milestoneId)
        //{
        //    var e = await _context.ProjectMilestones.FindAsync(milestoneId);
        //    if (e == null) return null;

        //    return new ProjectMilestoneDto
        //    {
        //        MilestoneId = e.MilestoneId,
        //        ProjectId = e.ProjectId,
        //        MilestoneName = e.MilestoneName,
        //        MilestoneDescription = e.MilestoneDescription,
        //        MilestoneStartDate = e.MilestoneStartDate,
        //        MilestoneEndDate = e.MilestoneEndDate,
        //        MilestoneStatus = e.MilestoneStatus,
        //        Remarks = e.Remarks
        //    };
        //}
        // --------------- Duration/Delay calculation ---------------
        // Uses DateOnly. If your EF doesn't support DateOnly, convert to DateTime before using.
        private void CalculateDays(ProjectTask entity)
        {
            // Use local today so calculations match user date
            var today = DateOnly.FromDateTime(DateTime.Now);

            // -------------------- DURATION --------------------
            if (entity.StartDate.HasValue)
            {
                // If finished → use FinishedDate, else use today
                var endDate = entity.FinishedDate ?? today;

                // Calculate difference
                int diff = (endDate.ToDateTime(TimeOnly.MinValue)
                           - entity.StartDate.Value.ToDateTime(TimeOnly.MinValue)).Days;

                // If end < start → task NOT started yet → duration = null
                entity.DurationDays = diff >= 0 ? diff : (int?)null;
            }
            else
            {
                // No start → no duration
                entity.DurationDays = null;
            }

            // -------------------- DELAY --------------------
            if (entity.PlannedEndDate.HasValue)
            {
                // If finished, compare with finished date; else compare with today
                var compareDate = entity.FinishedDate ?? today;

                int delay = (compareDate.ToDateTime(TimeOnly.MinValue)
                            - entity.PlannedEndDate.Value.ToDateTime(TimeOnly.MinValue)).Days;

                // Delay cannot be negative (early finish or on-time)
                entity.DelayedDays = delay > 0 ? delay : 0;
            }
            else
            {
                entity.DelayedDays = null;
            }
        }


        public async Task<bool> CreateTaskListAsync(List<ProjectTaskDto> dtoList)
        {
            foreach (var dto in dtoList)
            {
                var entity = new ProjectTask
                {
                    MilestoneId = dto.MilestoneId,
                    TaskCode = await GenerateTaskCodeAsync(),
                    TaskName = dto.TaskName,
                    StartDate = ToDateOnly(dto.StartDate),
                    PlannedEndDate = ToDateOnly(dto.PlannedEndDate),
                    FinishedDate = ToDateOnly(dto.FinishedDate),
                    Status = dto.Status,
                    Remarks = dto.Remarks,

                    Unit = dto.Unit,
                    TotalScope = dto.TotalScope,
                    ExecutedWork = dto.ExecutedWork,
                    Location = dto.Location,

                    CreatedAt = DateTime.UtcNow
                };

                CalculateDays(entity);
                _context.ProjectTasks.Add(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> UpdateTasksAsync(List<ProjectTaskDto> tasks)
        {
            foreach (var dto in tasks)
            {
                var entity = await _context.ProjectTasks.FindAsync(dto.TaskId);
                if (entity == null) continue;

                entity.TaskCode = dto.TaskCode;
                entity.TaskName = dto.TaskName;
                entity.StartDate = ToDateOnly(dto.StartDate);
                entity.PlannedEndDate = ToDateOnly(dto.PlannedEndDate);
                entity.FinishedDate = ToDateOnly(dto.FinishedDate);
                entity.Status = dto.Status;
                entity.Remarks = dto.Remarks;

                entity.Unit = dto.Unit;
                entity.TotalScope = dto.TotalScope;
                entity.ExecutedWork = dto.ExecutedWork;
                entity.Location = dto.Location;

                entity.UpdatedAt = DateTime.UtcNow;
                CalculateDays(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }


        // Delete with validation: if task has started, do not delete and return warning
        public async Task<BaseResponse> DeleteTaskAsync(int taskId)
        {
            var response = new BaseResponse();
            var entity = await _context.ProjectTasks.FindAsync(taskId);
            if (entity == null)
            {
                response.Success = false;
                response.Message = "Task not found.";
                response.Data = false;
                return response;
            }

            // Consider task started if StartDate not null OR status != null and not zero (0 = not started)
            bool started =
      entity.StartDate != null &&
      entity.Status != null &&
      entity.Status != 0;


            if (started)
            {
                response.Success = false;
                response.Message = "Cannot delete task because it has already started.";
                response.Data = false;
                return response;
            }

            _context.ProjectTasks.Remove(entity);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Task deleted successfully.";
            response.Data = true;
            return response;
        }
        public async Task<BaseResponse> DeleteMilestoneAsync(int milestoneId)
        {
            var response = new BaseResponse();

            // 1️⃣ Find milestone
            var milestone = await _context.ProjectMilestones
                .FirstOrDefaultAsync(m => m.MilestoneId == milestoneId);

            if (milestone == null)
            {
                response.Success = false;
                response.Message = "Milestone not found.";
                return response;
            }

            // 2️⃣ Check for tasks under this milestone
            var tasks = await _context.ProjectTasks
                .Where(t => t.MilestoneId == milestoneId)
                .ToListAsync();

            // 3️⃣ Check if any task is STARTED
            bool anyStarted = tasks.Any(t =>
                (t.StartDate != null) ||
                (t.Status != null && t.Status != 0)
            );

            if (anyStarted)
            {
                response.Success = false;
                response.Message = "Milestone cannot be deleted because one or more tasks have already started.";
                return response;
            }

            // 4️⃣ Delete tasks (optional if FK has cascade delete)
            _context.ProjectTasks.RemoveRange(tasks);

            // 5️⃣ Delete milestone
            _context.ProjectMilestones.Remove(milestone);

            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Milestone deleted successfully.";
            return response;
        }
        private async Task<string> GenerateTaskCodeAsync()
        {
            var last = await _context.ProjectTasks
                .OrderByDescending(t => t.TaskId)
                .FirstOrDefaultAsync();

            int next = (last?.TaskId ?? 0) + 1;

            return $"TASK{next}";
        }
        private async Task<string> GenerateSubTaskCodeAsync()
        {
            var last = await _context.ProjectSubtasks
                .OrderByDescending(t => t.SubtaskId)
                .FirstOrDefaultAsync();

            int next = (last?.SubtaskId ?? 0) + 1;

            return $"SUB{next}";
        }

        private static DateTime? NormalizeDate(dynamic date)
        {
            if (date == null) return null;
            if (date is string str)
            {
                if (string.IsNullOrWhiteSpace(str)) return null;
                if (DateTime.TryParse(str, out var parsed)) return parsed;
                return null; // cannot parse
            }
            return (DateTime?)date;
        }

        public async Task<bool> CreateSubTaskListAsync(List<ProjectSubTaskDto> dtoList)
        {
            foreach (var dto in dtoList)
            {
                var entity = new ProjectSubtask
                {
                    TaskId = dto.TaskId,
                    SubtaskCode = await GenerateSubTaskCodeAsync(),
                    SubtaskName = dto.SubtaskName,
                    StartDate = ToDateOnly(dto.StartDate),
                    PlannedEndDate = ToDateOnly(dto.PlannedEndDate),
                    FinishedDate = ToDateOnly(dto.FinishedDate),
                    Status = dto.Status,
                    Remarks = dto.Remarks,

                    Unit = dto.Unit,
                    TotalScope = dto.TotalScope,
                    ExecutedWork = dto.ExecutedWork,
                    Location = dto.Location,

                    CreatedAt = DateTime.UtcNow
                };

                CalculateDays(entity);
                _context.ProjectSubtasks.Add(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }



        public async Task<bool> UpdateSubTasksAsync(List<ProjectSubTaskDto> tasks)
        {
            foreach (var dto in tasks)
            {
                var entity = await _context.ProjectSubtasks.FindAsync(dto.TaskId);
                if (entity == null) continue;

                entity.SubtaskCode = dto.SubtaskCode;
                entity.SubtaskName = dto.SubtaskName;
                entity.StartDate = ToDateOnly(dto.StartDate);
                entity.PlannedEndDate = ToDateOnly(dto.PlannedEndDate);
                entity.FinishedDate = ToDateOnly(dto.FinishedDate);
                entity.Status = dto.Status;
                entity.Remarks = dto.Remarks;

                entity.Unit = dto.Unit;
                entity.TotalScope = dto.TotalScope;
                entity.ExecutedWork = dto.ExecutedWork;
                entity.Location = dto.Location;

                entity.UpdatedAt = DateTime.UtcNow;
                CalculateDays(entity);
            }

            return await _context.SaveChangesAsync() > 0;
        }


        // Delete with validation: if task has started, do not delete and return warning
        public async Task<BaseResponse> DeleteSubTaskAsync(int subtaskId)
        {
            var response = new BaseResponse();
            var entity = await _context.ProjectSubtasks.FindAsync(subtaskId);
            if (entity == null)
            {
                response.Success = false;
                response.Message = "Task not found.";
                response.Data = false;
                return response;
            }

            // Consider task started if StartDate not null OR status != null and not zero (0 = not started)
            bool started =
      entity.StartDate != null &&
      entity.Status != null &&
      entity.Status != 0;


            if (started)
            {
                response.Success = false;
                response.Message = "Cannot delete task because it has already started.";
                response.Data = false;
                return response;
            }

            _context.ProjectSubtasks.Remove(entity);
            await _context.SaveChangesAsync();

            response.Success = true;
            response.Message = "Task deleted successfully.";
            response.Data = true;
            return response;
        }

        private void CalculateDays(ProjectSubtask entity)
        {
            // Use local today so calculations match user date
            var today = DateOnly.FromDateTime(DateTime.Now);

            // -------------------- DURATION --------------------
            if (entity.StartDate.HasValue)
            {
                // If finished → use FinishedDate, else use today
                var endDate = entity.FinishedDate ?? today;

                // Calculate difference
                int diff = (endDate.ToDateTime(TimeOnly.MinValue)
                           - entity.StartDate.Value.ToDateTime(TimeOnly.MinValue)).Days;

                // If end < start → task NOT started yet → duration = null
                entity.DurationDays = diff >= 0 ? diff : (int?)null;
            }
            else
            {
                // No start → no duration
                entity.DurationDays = null;
            }

            // -------------------- DELAY --------------------
            if (entity.PlannedEndDate.HasValue)
            {
                // If finished, compare with finished date; else compare with today
                var compareDate = entity.FinishedDate ?? today;

                int delay = (compareDate.ToDateTime(TimeOnly.MinValue)
                            - entity.PlannedEndDate.Value.ToDateTime(TimeOnly.MinValue)).Days;

                // Delay cannot be negative (early finish or on-time)
                entity.DelayedDays = delay > 0 ? delay : 0;
            }
            else
            {
                entity.DelayedDays = null;
            }
        }
        public async Task<List<UnitMasterDto>> GetAllUnitMasterAsync()
        {
            var units = await _context.UnitMasters.ToListAsync();

            return units.Select(u => new UnitMasterDto
            {
                Id = u.Id,
                Name = u.Name,
                Code = u.Code
            }).ToList();
        }
        public async Task<List<MilestoneSummaryDto>> GetMilestoneSummaryAsync(int projectId)
        {
            var statusMaster = await _context.Taskstatusmasters.ToListAsync();

            string TODO = statusMaster.First(x => x.Code == "TODO").Code;
            string PROGRESS = statusMaster.First(x => x.Code == "PROGRESS").Code;
            string COMPLETED = statusMaster.First(x => x.Code == "COMPLETED").Code;
            string FAIL = statusMaster.First(x => x.Code == "FAIL").Code;

            var milestones = await _context.ProjectMilestones
                .Where(m => m.ProjectId == projectId)
                .Include(m => m.ProjectTasks)
                .ToListAsync();

            var result = milestones.Select(m =>
            {
                var tasks = m.ProjectTasks;

                int totalScope = tasks.Sum(t => t.TotalScope ?? 0);
                int executedWork = tasks.Sum(t => t.ExecutedWork ?? 0);
                int balanceScope = totalScope - executedWork;

                var taskStatusCodes = tasks
                    .Where(t => t.Status != null)
                    .Select(t => statusMaster.First(s => s.Id == t.Status).Code)
                    .ToList();

                string milestoneStatus = "Pending";

                if (!taskStatusCodes.Any())
                    milestoneStatus = "Pending";
                else if (taskStatusCodes.All(c => c == COMPLETED))
                    milestoneStatus = "Completed";
                else if (taskStatusCodes.All(c => c == FAIL))
                    milestoneStatus = "Failed";
                else if (taskStatusCodes.Any(c => c == TODO || c == PROGRESS))
                    milestoneStatus = "InProgress";

                var finishedDate = tasks
                    .Where(t => t.Status != null &&
                                statusMaster.First(s => s.Id == t.Status).Code == COMPLETED &&
                                t.FinishedDate != null)
                    .Max(t => t.FinishedDate);

                int durationDays = 0;
                int delayDays = 0;

                if (m.MilestoneStartDate.HasValue && m.MilestoneEndDate.HasValue)
                {
                    durationDays = (m.MilestoneEndDate.Value.ToDateTime(TimeOnly.MinValue)
                                   - m.MilestoneStartDate.Value.ToDateTime(TimeOnly.MinValue)).Days;
                }

                if (finishedDate.HasValue && m.MilestoneEndDate.HasValue)
                {
                    delayDays = Math.Max(0,
                        (finishedDate.Value.ToDateTime(TimeOnly.MinValue)
                        - m.MilestoneEndDate.Value.ToDateTime(TimeOnly.MinValue)).Days);
                }

                return new MilestoneSummaryDto
                {
                    MilestoneId = m.MilestoneId,
                    MilestoneName = m.MilestoneName,
                    Location = tasks.FirstOrDefault()?.Location,
                    Remarks = m.Remarks,
                    StartDate = m.MilestoneStartDate,
                    EndDate = m.MilestoneEndDate,
                    FinishedDate = finishedDate,
                    TotalScope = totalScope,
                    ExecutedWork = executedWork,
                    BalanceScope = balanceScope,
                    CompletedStatus = milestoneStatus,
                    DurationDays = durationDays,
                    DelayDays = delayDays
                };
            }).ToList();

            return result;
        }

    }
}
