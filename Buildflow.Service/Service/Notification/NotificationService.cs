using Buildflow.Library.UOW;
using Buildflow.Utility.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Buildflow.Service.Service.Notification
{
    public class NotificationService
    {

        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<List<NotificationDto>> CreateNotificationAsync(NotificationInput createDto)
        {
            return await _unitOfWork.NotificationRepository.CreateNotificationAsync(createDto);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(NotificationData queryDto)
        {
            return await _unitOfWork.NotificationRepository.GetNotificationsAsync(queryDto);
        }


        public async Task MarkAsReadAsync(MarkAsReadDto markAsReadDto)
        {
            // Delegate the work to the repository layer
            await _unitOfWork.NotificationRepository.MarkAsReadAsync(markAsReadDto);
        }
    }
}
