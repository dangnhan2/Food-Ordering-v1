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
        public Task<Result<IEnumerable<CategoryDTO>>> GetAllAsync();
        public Task<Result<Categories>> AddAsync(CategoryRequest request);
        public Task<Result<Categories>> UpdateAsync(Guid id, CategoryRequest request);
        public Task<Result<Categories>> DeleteAsync(Guid id);
    }
}
