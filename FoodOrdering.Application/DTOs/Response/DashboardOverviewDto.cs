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
        public int RevenueToday { get; set; }
        public int TotalCustomers { get; set; }
        public int NewCustomersToday { get; set; }
        public int TotalMenuItems { get; set; }
        public ICollection<TopDishDto> TopSellingDishes { get; set; } = new List<TopDishDto>();
    }
}
