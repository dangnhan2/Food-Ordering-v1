using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface ICartRepo : IGenericRepo<Carts>
    {
        public Task<Carts?> GetCartWithCartItemAsync(Guid id);
        public void DeleteExpiredCart(IEnumerable<Carts> carts);
    }
}
