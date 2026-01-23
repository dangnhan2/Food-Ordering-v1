using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface IMenuRepo : IGenericRepo<Menu>
    {
        public Task<MenuDto?> GetMenuWithCategoryAsync(Guid id);
    }
}
