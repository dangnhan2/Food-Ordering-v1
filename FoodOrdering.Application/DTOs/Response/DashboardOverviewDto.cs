using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Response
{
    public class DashboardOverviewDTO
    {
        public int TotalOrdersToday { get; set; }
        public int PaidOrdersToday { get; set; }
        public int CancelledOrdersToday { get; set; }
        public decimal RevenueToday { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalMenuItems { get; set; }
        public decimal RevenueMonthly { get; set; }
        public int TotalPaidOrdersMontly { get; set; }
        public ICollection<TopDishDto> TopSellingDishes { get; set; } = new List<TopDishDto>();
        public IEnumerable<TopBuyerDto> TopBuyers { get; set; } = new List<TopBuyerDto>();
    }
}
