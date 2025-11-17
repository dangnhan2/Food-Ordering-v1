using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.Services.Interface
{
    public interface INotificationService
    {
        public Task<IEnumerable<NotificationDto>> GetUnreadByAdmin(Guid id);
        public Task MarkAsReadAsync(List<Guid> ids);
    }

    public interface INotificationSenderService
    {
        public Task NotifyAdminAsync(int orderCode);
    }
}
