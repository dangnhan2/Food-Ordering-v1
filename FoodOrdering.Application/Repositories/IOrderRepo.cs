using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface IOrderRepo : IGenericRepo<Orders>
    {
        public Task<Orders?> GetOrderByOrderCode(int code);
    }
}
