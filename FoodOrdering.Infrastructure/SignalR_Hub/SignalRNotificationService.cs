using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Infrastructure.SignalR_Hub
{
    public class SignalRNotificationService : INotificationService
    {   
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task<ApiResponse<Notification>> MarkAsReadAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task NotifyAdminAsync(string message)
        {
            await _hubContext.Clients.Group("Admins")
                .SendAsync(message);
        }
    }
}
