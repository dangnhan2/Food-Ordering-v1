using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Notification>> MarkAsReadAsync(Guid id)
        {
            var notification = await _unitOfWork.Notification.GetByIdAsync(id);

            if (notification == null)
                return ApiResponse<Notification>.Fail("Không tìm thấy thông báo", StatusCodes.Status404NotFound);

            notification.IsRead = true;
            _unitOfWork.Notification.Update(notification);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Notification>.Success("Cập nhật thông báo thành công", notification, StatusCodes.Status200OK);
        }
    }
}
