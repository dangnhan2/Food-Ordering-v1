using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachingService _cacheService;

        public CategoryService(IUnitOfWork unitOfWork, ICachingService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {   
            string cacheKey = CacheKeys.CATEGORIES_PREFIX;
            var cachedCategories = await _cacheService.GetAsync<IEnumerable<CategoryDTO>>(cacheKey);

            if (cachedCategories != null)
                return cachedCategories;

            var categories = _unitOfWork.Category.GetAll();

            var categoriesToDTO = await categories
                .OrderBy(c => c.Name)
                .Select(c => new CategoryDTO(c))
                .AsNoTracking()
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, categoriesToDTO, TimeSpan.FromMinutes(30));

            return categoriesToDTO;
        }

        public async Task AddAsync(CategoryRequestDto request)
        {
            var result = await new CategoryValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());
             
            var categories = _unitOfWork.Category
                .GetAll()
                .Where(c => EF.Functions.Like(c.Name, $"%{request.Name}%"));

            if (categories.Any())
                throw new DuplicateNameException($"{request.Name} đã tồn tại");

            var newCategory = MappingCategory(request);

            await _unitOfWork.Category.AddAsync(newCategory);
            await _unitOfWork.SaveChangeAsync();

            // delete old cache
            await _cacheService.RemoveAsync(CacheKeys.CATEGORIES_PREFIX);
        }

        public async Task UpdateAsync(Guid id, CategoryRequestDto request)
        {
            var validator = new CategoryValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var categories = _unitOfWork.Category
                .GetAll()
                .Where(c => c.Name.Trim().ToLower() ==  request.Name.Trim().ToLower() && c.Id != id);

            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException("Danh mục không tồn tại");

            if (categories.Any())
                throw new DuplicateNameException($"{request.Name} đã tồn tại");

            category.Name = request.Name;

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(CacheKeys.CATEGORIES_PREFIX);
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)           
               throw new KeyNotFoundException("Danh mục không tồn tại");

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(CacheKeys.CATEGORIES_PREFIX);
        }

        private Category MappingCategory(CategoryRequestDto request)
        {
            Category category = new Category
            {
                Name = request.Name,
            };

            return category;
        }
    }
}
