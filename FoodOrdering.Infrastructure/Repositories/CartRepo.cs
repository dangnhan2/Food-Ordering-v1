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
    public class CartRepo : GenericRepo<Cart>, ICartRepo
    {
        private readonly FoodOrderingDbContext _context;
        public CartRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public void DeleteExpiredCart(IEnumerable<Cart> carts)
        {
            _context.Cart.RemoveRange(carts);
        }

        public async Task<Cart?> GetCartByCustomerAsync(Guid id)
        {
            return await _context.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ct => ct.Menu)
                .FirstOrDefaultAsync(c => c.UserId == id);
        }

        public async Task<Cart?> GetCartWithCartItemAsync(Guid id)
        {
            return await _context.Cart.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}
