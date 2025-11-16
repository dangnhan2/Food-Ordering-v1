using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.Services.Interface
{
    public interface INotificationService
    {
        public Task NotifyAdminAsync(string message);
        public Task<ApiResponse<Notification>> MarkAsReadAsync(Guid id);
    }
}
