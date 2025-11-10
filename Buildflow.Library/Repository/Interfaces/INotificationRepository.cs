using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Library.Repository.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<NotificationDto>> CreateNotificationAsync(NotificationInput createDto);
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync(NotificationData queryDto);
        Task MarkAsReadAsync(MarkAsReadDto markAsReadDto);
    }
}
