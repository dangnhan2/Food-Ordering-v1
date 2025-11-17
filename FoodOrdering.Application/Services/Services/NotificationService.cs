using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace FoodOrdering.Application.Services.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadByAdmin(Guid id)
        {
            var notifications = _unitOfWork.Notification
                .GetAll()
                .Where(x => x.UserId == id && !x.IsRead);

            var notificationToDto = await notifications
                .Select(n => new NotificationDto
                {
                    Id = id,
                    Tiltle = n.Tiltle,
                    Message = n.Message,
                    Type = n.Type,
                    Data = n.Data,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt
                }).ToListAsync();

            return notificationToDto;
        }

        public async Task MarkAsReadAsync(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                var existNotification = await _unitOfWork.Notification.GetByIdAsync(id);

                if (existNotification == null) continue;

                existNotification.IsRead = true;
            }

            await _unitOfWork.SaveChangeAsync();
        }
    }
}
