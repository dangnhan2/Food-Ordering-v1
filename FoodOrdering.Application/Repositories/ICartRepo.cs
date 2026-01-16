using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface ICartRepo : IGenericRepo<Cart>
    {
        public Task<Cart?> GetCartByCustomerAsync(Guid userId);
        public Task<Cart?> GetCartWithCartItemAsync(Guid id);
        public void DeleteExpiredCart(IEnumerable<Cart> carts);
    }
}
