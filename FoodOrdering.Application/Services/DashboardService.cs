using Food_Ordering.Models.Enum;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class DashboardService : IDashboardService
    {   
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<DashboardOverviewDTO>> GetInfoAsync()
        {
            var today = DateTime.UtcNow.Date;

            var totalOrders = _unitOfWork.Order.GetAll().Count(o => o.OrderDate.Date == today);
            var paidOrders = _unitOfWork.Order.GetAll();
            var cancelledOrders = _unitOfWork.Order.GetAll().Count(o => o.Status == OrderStatus.Cancelled && o.OrderDate.Date == today);
            var newCustomersToday = _unitOfWork.User.GetAll().Count(u => u.CreatedAt.Date == today);
            var totalPaidOrders = paidOrders.Count(o => o.Status == OrderStatus.Paid && o.OrderDate.Date == today);
            var totalMenuItems = _unitOfWork.Menu.GetAll().Count();
            var totalUsers = _unitOfWork.User.GetAll().Count();

            var totalAmount = paidOrders.Sum(o => o.ToTalAmount);

            var topSellingDishes = await _unitOfWork.Menu
                .GetAll()
                .OrderByDescending(m => m.SoldQuantity)
                .Take(3)
                .Select(d => new TopDishDto(d))
                .AsNoTracking()
                .ToListAsync();

            var dashboardToDTO = new DashboardOverviewDTO
            {
                TotalOrdersToday = totalOrders,
                PaidOrdersToday = totalPaidOrders,
                CancelledOrdersToday = cancelledOrders,
                RevenueToday = totalAmount,
                TotalCustomers = totalUsers,
                TotalMenuItems = totalMenuItems,
                NewCustomersToday = newCustomersToday,
                TopSellingDishes = topSellingDishes
            };

            return ApiResponse<DashboardOverviewDTO>.Success("Lấy dữ liệu thành công", dashboardToDTO, 200);
        }
    }
}
