
using FoodOrdering.Application;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace FoodOrdering.Infrastructure.SignalR_Hub
{
    public class SignalRNotificationService : INotificationSenderService
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly UserManager<User> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public SignalRNotificationService(
            IHubContext<NotificationHub> hubContext, 
            UserManager<User> userManager,
            IUnitOfWork unitOfWork)
        {
            _hubContext = hubContext;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task NotifyAdminAsync(int orderCode)
        {
            var users = await _userManager.GetUsersInRoleAsync("Admin");
            List<Notification> notifications = new List<Notification>();

            foreach (var user in users)
            {
               var newNotification = MappingNotification(user.Id, orderCode);
               notifications.Add(newNotification);

                // send notification to group admins
                await _hubContext.Clients.Group("Admins")
                    .SendAsync("ReceiveNotification", new NotificationDto
                    {
                        Id = newNotification.Id,
                        Tiltle = "Bạn có đơn hàng mới",
                        Message = $"Đơn hàng #{orderCode} vừa được tạo",
                        Type = "Order",
                        Data = "",
                        IsRead = false
                    });
            }

            await _unitOfWork.Notification.AddRangeAsync(notifications);
            await _unitOfWork.SaveChangeAsync();
        }

        private Notification MappingNotification(Guid userId, int orderCode)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Tiltle = "Bạn có đơn hàng mới",
                Message = $"Đơn hàng #{orderCode} vừa đặt thành công",
                Type = "Order",
                Data = "",
                IsRead = false
            };

            return notification;
        }
    }
}
