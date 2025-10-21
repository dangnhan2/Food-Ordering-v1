using FoodOrdering.Application.Caching;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;

        public CategoryService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<ApiResponse<IEnumerable<CategoryDTO>>> GetAllAsync()
        {   
            // check cache
            string cacheKey = "categories:all";
            var cachedCategories = await _cacheService.GetAsync<IEnumerable<CategoryDTO>>(cacheKey);

            if (cachedCategories != null) 
                return ApiResponse<IEnumerable<CategoryDTO>>.Success("Lấy dữ liệu thành công", cachedCategories, StatusCodes.Status200OK);

            // if not, get from db
            var categories = _unitOfWork.Category.GetAll();

            var categoriesToDTO = await categories.Select(c => new CategoryDTO(c)).AsNoTracking().ToListAsync();

            await _cacheService.SetAsync(cacheKey, categoriesToDTO, TimeSpan.FromMinutes(30));

            return ApiResponse<IEnumerable<CategoryDTO>>.Success("Lấy dữ liệu thành công", categoriesToDTO, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<Categories>> AddAsync(CategoryRequest request)
        {
            var result = await new CategoryValidator().ValidateAsync(request);

            if (!result.IsValid)          
               return ApiResponse<Categories>.Fail(result.ToDictionary(), 400);

            var categories = _unitOfWork.Category.GetAll().Where(c => c.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            if (categories.Any())
                return ApiResponse<Categories>.Fail($"{request.Name} đã tồn tại", StatusCodes.Status400BadRequest);

            Categories category= new Categories
            {
                Name = request.Name,
            };

            await _unitOfWork.Category.AddAsync(category);
            await _unitOfWork.SaveChangeAsync();

            // delete old cache
            await _cacheService.RemoveAsync("categories:all");

            return ApiResponse<Categories>.Success($"Thêm menu {request.Name} thành công", category, StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<Categories>> UpdateAsync(Guid id, CategoryRequest request)
        {
            var validator = new CategoryValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)           
              return ApiResponse<Categories>.Fail(result.ToDictionary(), 400);
                         
            var categories = _unitOfWork.Category.GetAll().Where(c => c.Name.Trim().ToLower() ==  request.Name.Trim().ToLower() && c.Id != id);
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)
                return ApiResponse<Categories>.Fail("Không tìm thấy menu", 404);

            if (categories.Any())
                return ApiResponse<Categories>.Fail($"Menu {request.Name} đã tồn tại", 400);

            category.Name = request.Name;

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync("categories:all");

            return ApiResponse<Categories>.Success($"Cập nhật menu {request.Name} thành công", category, 200);
        }

        public async Task<ApiResponse<Categories>> DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)           
              return ApiResponse<Categories>.Fail("Không tìm thấy menu", 404);
            
            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync("categories:all");
            return ApiResponse<Categories>.Success($"Xóa menu {category.Name} thành công", category, 200);
        }
    }
}
