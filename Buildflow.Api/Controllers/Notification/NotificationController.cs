using Buildflow.Infrastructure.DatabaseContext;
using Buildflow.Service.Service.Notification;
using Buildflow.Service.Service.Project;
using Buildflow.Utility.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Buildflow.Api.Controllers.Notification
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly NotificationService _notificationService;
        private readonly IConfiguration _config;
        private readonly BuildflowAppContext _context;

        public NotificationController(NotificationService notificationService, IConfiguration config, BuildflowAppContext context)
        {
            _notificationService = notificationService;
            _config = config;
            _context = context;
        }
  
        [HttpPost("create-notification")]
        public async Task<ActionResult<List<NotificationDto>>> CreateNotification([FromBody] NotificationInput createDto)
        {
            var result = await _notificationService.CreateNotificationAsync(createDto);
            return Ok(result); 
        }

        [HttpGet("get-notification")]
       
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetNotifications([FromQuery] NotificationData queryDto)
        {
            var notifications = await _notificationService.GetNotificationsAsync(queryDto);
            return Ok(notifications);
        }

        [HttpPut("markasread")]
        public async Task<IActionResult> MarkAsRead(MarkAsReadDto markAsReadDto)
        {
            await _notificationService.MarkAsReadAsync(markAsReadDto);
            return Ok(new { message = "Notifications marked as read successfully." }); // Or return the updated notifications
        }







    }
}
