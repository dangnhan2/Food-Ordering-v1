using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.Services.Interface
{
    public interface INotificationService
    {
        public Task<UnreadNotificationDto> GetUnReadNotificationsByAdmin(Guid adminId);
        public Task<IEnumerable<NotificationDto>> GetNotificationsByAdmin(Guid adminId);
        public Task MarkAsReadAsync(MarkNotificationRequestDto notificationIds);
        public Task DeleteAsync(Guid id);
    }
}
