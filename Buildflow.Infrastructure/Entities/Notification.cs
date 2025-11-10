using System;
using System.Collections.Generic;

namespace Buildflow.Infrastructure.Entities;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int? EmpId { get; set; }

    public string? NotificationType { get; set; }

    public int? SourceEntityId { get; set; }

    public string? Message { get; set; }

    public DateTime? Timestamp { get; set; }

    public bool? IsRead { get; set; }

    public virtual EmployeeDetail? Emp { get; set; }
}
