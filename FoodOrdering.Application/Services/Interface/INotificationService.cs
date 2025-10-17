using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface INotificationService
    {
        public Task<ApiResponse<Notification>> MarkAsReadAsync(Guid id);
    }
}
