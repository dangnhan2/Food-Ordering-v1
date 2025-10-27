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
using System.Data;
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

        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {   
            // check cache
            string cacheKey = "categories:all";
            var cachedCategories = await _cacheService.GetAsync<IEnumerable<CategoryDTO>>(cacheKey);

            if (cachedCategories != null)
                return cachedCategories;

            // if not, get from db
            var categories = _unitOfWork.Category.GetAll();

            var categoriesToDTO = await categories.Select(c => new CategoryDTO(c)).AsNoTracking().ToListAsync();

            await _cacheService.SetAsync(cacheKey, categoriesToDTO, TimeSpan.FromMinutes(30));

            return categoriesToDTO;
        }

        public async Task AddAsync(CategoryRequest request)
        {
            var result = await new CategoryValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());
             
            var categories = _unitOfWork.Category.GetAll().Where(c => c.Name.Trim().ToLower() == request.Name.Trim().ToLower());

            if (categories.Any())
                throw new DuplicateNameException($"{request.Name} đã tồn tại");
                
            Categories category= new Categories
            {
                Name = request.Name,
            };

            await _unitOfWork.Category.AddAsync(category);
            await _unitOfWork.SaveChangeAsync();

            // delete old cache
            await _cacheService.RemoveAsync("categories:all");
        }

        public async Task UpdateAsync(Guid id, CategoryRequest request)
        {
            var validator = new CategoryValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var categories = _unitOfWork.Category.GetAll().Where(c => c.Name.Trim().ToLower() ==  request.Name.Trim().ToLower() && c.Id != id);
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)
                throw new KeyNotFoundException(nameof(category));

            if (categories.Any())
                throw new DuplicateNameException($"{request.Name} đã tồn tại");

            category.Name = request.Name;

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync("categories:all");
        }

        public async Task DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)           
               throw new KeyNotFoundException(nameof(category));

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync("categories:all");
        }
    }
}
