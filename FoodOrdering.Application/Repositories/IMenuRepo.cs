using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface IMenuRepo : IGenericRepo<Menus>
    {
        public Task<Menus?> GetMenuWithCategoryAsync(Guid id);
    }
}
