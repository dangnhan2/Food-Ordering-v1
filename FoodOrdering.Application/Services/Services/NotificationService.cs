using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Helper.Extensions;
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

        public async Task DeleteAsync(Guid id)
        {
            var existNotification = await _unitOfWork.Notification.GetByIdAsync(id);

            if (existNotification == null) throw new KeyNotFoundException("Thông báo không tồn tại");

            _unitOfWork.Notification.Remove(existNotification);

            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByAdmin(Guid id)
        {
            var notifications = _unitOfWork.Notification
                .GetAll();

            var notificationToDto = await notifications
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking()
                .Select(n => new NotificationDto
                {
                    Id = n.Id,
                    Tiltle = n.Tiltle,
                    Message = n.Message,
                    Type = n.Type,
                    Data = n.Data,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt.FormatDateTime()
                }).ToListAsync();

            return notificationToDto;
        }

        public async Task MarkAsReadAsync(MarkNotificationRequestDto notificationIds)
        {
            foreach (var id in notificationIds.NotificationIds)
            {
                var existNotification = await _unitOfWork.Notification.GetByIdAsync(id);

                if (existNotification == null) continue;

                existNotification.IsRead = true;
            }
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
