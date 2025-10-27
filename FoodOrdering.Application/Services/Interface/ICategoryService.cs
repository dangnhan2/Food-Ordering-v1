using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface ICategoryService
    {
        public Task<IEnumerable<CategoryDTO>> GetAllAsync();
        public Task AddAsync(CategoryRequest request);
        public Task UpdateAsync(Guid id, CategoryRequest request);
        public Task DeleteAsync(Guid id);
    }
}
