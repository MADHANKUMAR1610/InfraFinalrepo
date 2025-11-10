using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Utility.DTO
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public int EmpId { get; set; }
        public string NotificationType { get; set; }
        public int SourceEntityId { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsRead { get; set; }

    }

    public class NotificationInput
    {
        
        public int[] EmpId { get; set; }

        
        public string NotificationType { get; set; }

        public int SourceEntityId { get; set; }

        
        public string Message { get; set; }

    }

    public class NotificationData
    {
        public int? UserId { get; set; }
        public string? NotificationType { get; set; }
        public bool? IsRead { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } = "Timestamp"; // Default sort
        public string? SortDirection { get; set; } = "Desc"; // Default direction
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class MarkAsReadDto
    {
        
        public int EmpId { get; set; }

        public List<int> NotificationIds { get; set; } = new List<int>();

        public bool MarkAll { get; set; } = false;
    }


}
