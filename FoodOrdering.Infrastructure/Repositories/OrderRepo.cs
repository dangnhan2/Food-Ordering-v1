using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repository
{
    public class OrderRepo : GenericRepo<Order>, IOrderRepo
    {   
        private readonly FoodOrderingDbContext _context;
        public OrderRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public async Task<Order?> GetOrderByOrderCode(int code)
        {
            return await _context.Order
                .Include(o => o.OrderMenus)
                .FirstOrDefaultAsync(o => o.OrderCode == code);
        }
    }
}
