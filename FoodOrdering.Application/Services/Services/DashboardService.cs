using Food_Ordering.Models.Enum;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Services
{
    public class DashboardService : IDashboardService
    {   
        private readonly IUnitOfWork _unitOfWork;
        public DashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<DashboardOverviewDTO> GetInfoAsync()
        {
            var today = DateTimeOffset.UtcNow.Date;
            var month = DateTimeOffset.UtcNow.Month;

            var totalOrdersToday = _unitOfWork.Order
                .GetAll()
                .Count(o => o.OrderDate.Date == today);

            var cancelledOrdersToday = _unitOfWork.Order
                .GetAll()
                .Count(o => o.Status == OrderStatus.Cancelled && o.OrderDate.Date == today);

            var paidOrders = _unitOfWork.Order
                .GetAll()
                .Where(o => o.Status == OrderStatus.Paid);

            var totalPaidOrdersToday = paidOrders
                .Count(o => o.Status == OrderStatus.Paid && o.OrderDate.Date == today);

            var totalMenuItems = _unitOfWork.Menu
                .GetAll()
                .Where(m => m.IsAvailable)
                .Count();

            var totalUsers = _unitOfWork.User
                .GetAll()
                .Where(u => !u.IsAdmin)
                .Count();

            var revenuePaidOrdersMonthly = _unitOfWork.Order
                .GetAll()
                .Where(u => u.Status == OrderStatus.Paid && u.OrderDate.Month == month)
                .Sum(o => o.TotalAmount);
    

            var totalAmount = paidOrders
                .Where(o => o.OrderDate.Date == today)
                .Sum(o => o.TotalAmount);

            // Get 5 the best dishes 
            var topSellingDishes = await _unitOfWork.Menu
                .GetAll()
                .OrderByDescending(m => m.SoldQuantity)
                .Take(5)
                .Select(d => new TopDishDto(d))
                .AsNoTracking()
                .ToListAsync();

            // Get paid orders monthly
            var totalPaidOrdersMonthly = _unitOfWork.Order
                .GetAll()
                .Where(o => o.Status == OrderStatus.Paid && o.OrderDate.Month == month)
                .Count();

            // Get 5 the most spenders monthly
            var topBuyers = await _unitOfWork.User
                .GetAll()
                .Take(5)
                .Where(u => !u.IsAdmin)
                .OrderByDescending(u => u.Orders.Sum(o => o.TotalAmount))
                .Select(u => new TopBuyerDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    TotalAmountInAMonth = u.Orders.Sum(o => o.TotalAmount)
                })
                .AsNoTracking()
                .ToListAsync();

            var dashboardToDTO = new DashboardOverviewDTO
            {
                TotalOrdersToday = totalOrdersToday,
                PaidOrdersToday = totalPaidOrdersToday,
                CancelledOrdersToday = cancelledOrdersToday,
                RevenueToday = totalAmount,
                TotalCustomers = totalUsers,
                TotalMenuItems = totalMenuItems,
                TopSellingDishes = topSellingDishes,
                RevenueMonthly = revenuePaidOrdersMonthly,
                TotalPaidOrdersMontly = totalPaidOrdersMonthly,
                TopBuyers = topBuyers
            };

            return dashboardToDTO;
        }
    }
}
