using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Infrastructure.Entities;
using Buildflow.Library.Repository.Interfaces;
using Buildflow.Utility.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository
{
    public class NotificationRepository:INotificationRepository
    {
        private readonly BuildflowAppContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GenericRepository<Notification>> _logger;

        public NotificationRepository(IConfiguration configuration, BuildflowAppContext context, ILogger<GenericRepository<Notification>> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }


        public IDbConnection CreateConnection() => new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));

        public async Task<List<NotificationDto>> CreateNotificationAsync(NotificationInput createDto)
        {
            try
            {
                var notifications = new List<Notification>();

                foreach (var empId in createDto.EmpId)
                {
                    var notification = new Notification
                    {
                        EmpId = empId,
                        NotificationType = createDto.NotificationType,
                        SourceEntityId = createDto.SourceEntityId,
                        Message = createDto.Message,
                        Timestamp = DateTime.UtcNow, // Set timestamp
                        IsRead = false // Default unread
                    };

                    notifications.Add(notification);
                }

                _context.Notifications.AddRange(notifications);
                await _context.SaveChangesAsync();

                // Prepare and return created notifications
                var result = notifications.Select(notification => new NotificationDto
                {
                    NotificationId = notification.NotificationId,
                    EmpId = notification.EmpId.Value,
                    NotificationType = notification.NotificationType,
                    SourceEntityId = notification.SourceEntityId.Value,
                    Message = notification.Message,
                    Timestamp = notification.Timestamp.Value,
                    IsRead = notification.IsRead.Value
                }).ToList();

                return result;
            }
            catch (Exception ex)
            {
                // Log exception here if you have a logger (optional)
                // _logger.LogError(ex, "Error creating notifications.");

                throw new Exception("An error occurred while creating notifications.");
            }
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(NotificationData queryDto)
        {
            IQueryable<Notification> query = _context.Notifications;

            // Apply filters
            if (queryDto.UserId.HasValue)
            {
                query = query.Where(n => n.EmpId == queryDto.UserId.Value);  // Use EmpId for UserId
            }

            if (!string.IsNullOrEmpty(queryDto.NotificationType))
            {
                query = query.Where(n => n.NotificationType == queryDto.NotificationType);
            }

            if (queryDto.IsRead.HasValue)
            {
                query = query.Where(n => n.IsRead == queryDto.IsRead.Value);
            }

            if (queryDto.StartDate.HasValue)
            {
                query = query.Where(n => n.Timestamp >= queryDto.StartDate.Value);
            }

            if (queryDto.EndDate.HasValue)
            {
                query = query.Where(n => n.Timestamp <= queryDto.EndDate.Value.AddDays(1).AddTicks(-1)); // Include end of day
            }

            // Sorting logic
            if (!string.IsNullOrEmpty(queryDto.SortBy))
            {
                query = queryDto.SortBy.ToLowerInvariant() switch
                {
                    "notificationtype" => queryDto.SortDirection?.ToLowerInvariant() == "desc" ? query.OrderByDescending(n => n.NotificationType) : query.OrderBy(n => n.NotificationType),
                    "timestamp" => queryDto.SortDirection?.ToLowerInvariant() == "desc" ? query.OrderByDescending(n => n.Timestamp) : query.OrderBy(n => n.Timestamp),
                    "isread" => queryDto.SortDirection?.ToLowerInvariant() == "desc" ? query.OrderByDescending(n => n.IsRead) : query.OrderBy(n => n.IsRead),
                    _ => query.OrderByDescending(n => n.Timestamp) // Default to timestamp descending
                };
            }
            else
            {
                query = query.OrderByDescending(n => n.Timestamp); // Default sorting by timestamp descending
            }

            // Apply pagination
            //var notifications = await query
            //    .Skip((queryDto.PageNumber - 1) * queryDto.PageSize)
            //    .Take(queryDto.PageSize)
            //    .ToListAsync();

            var notifications = await query.ToListAsync();


            // Map to DTO
            return notifications.Select(n => new NotificationDto
            {
                NotificationId = n.NotificationId,
                EmpId = n.EmpId.Value,
                NotificationType = n.NotificationType,
                SourceEntityId = n.SourceEntityId.Value,
                Message = n.Message,
                Timestamp = n.Timestamp.Value,
                IsRead = n.IsRead.Value
            });
        }

        public async Task MarkAsReadAsync(MarkAsReadDto markAsReadDto)
        {
            if (markAsReadDto.MarkAll)
            {
                // Mark all unread notifications for the user as read
                var notificationsToUpdate = await _context.Notifications
                    .Where(n => n.EmpId == markAsReadDto.EmpId && (n.IsRead==false))
                    .ToListAsync();

                foreach (var notification in notificationsToUpdate)
                {
                    notification.IsRead = true;
                }
            }
            else if (markAsReadDto.NotificationIds != null && markAsReadDto.NotificationIds.Any())
            {
                // Mark specific notifications as read
                var notificationsToUpdate = await _context.Notifications
                    .Where(n => n.EmpId == markAsReadDto.EmpId && markAsReadDto.NotificationIds.Contains(n.NotificationId))
                    .ToListAsync();

                foreach (var notification in notificationsToUpdate)
                {
                    notification.IsRead = true;
                }
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
        }
    }
}
