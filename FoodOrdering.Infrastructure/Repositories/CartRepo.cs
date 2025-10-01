using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class CartRepo : GenericRepo<Carts>, ICartRepo
    {
        private readonly FoodOrderingDbContext _context;
        public CartRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public void DeleteExpiredCart(IEnumerable<Carts> carts)
        {
            _context.Carts.RemoveRange(carts);
        }

        public async Task<Carts?> GetCartByCustomerAsync(Guid id)
        {
            return await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ct => ct.Menu)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<Carts?> GetCartWithCartItemAsync(Guid id)
        {
            return await _context.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
