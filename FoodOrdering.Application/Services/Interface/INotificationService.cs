using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.Services.Interface
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDto>> GetNotificationsByAdmin(Guid id);
        public Task MarkAsReadAsync(MarkNotificationRequestDto notificationIds);
        public Task DeleteAsync(Guid id);
    }

    public interface INotificationSenderService
    {
        public Task NotifyAdminAsync(int orderCode, decimal totalAmount);
    }
}
