using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FoodOrdering.Application.Services.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachingRepo _cacheService;

        public CategoryService(IUnitOfWork unitOfWork, ICachingRepo cacheService)
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
                .Select(c => new CategoryDTO
                {  
                    Id = c.Id,
                    Name = c.Name
                })
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
             
            var isCategoryExist = _unitOfWork.Category
                .GetAll()
                .Any(c => c.Name.Contains(request.Name));

            if (isCategoryExist)
                throw new DuplicateNameException($"{request.Name} đã tồn tại");

            var newCategory = MappingCategory(request);

            await _unitOfWork.Category.AddAsync(newCategory);
            await _unitOfWork.SaveChangeAsync();

            // delete old cache
            await _cacheService.RemoveAsync(CacheKeys.CATEGORIES_PREFIX);
        }

        public async Task UpdateAsync(Guid categoryId, CategoryRequestDto request)
        {
            var result = await new CategoryValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var category = await _unitOfWork.Category.GetByIdAsync(categoryId);

            if (category == null)
                throw new KeyNotFoundException("Danh mục không tồn tại");

            var isCategoryExist = _unitOfWork.Category
                .GetAll()
                .Any(c => c.Name.Contains(request.Name) && c.Id != categoryId);       
         
            if (isCategoryExist)
                throw new DuplicateNameException($"{request.Name} đã tồn tại");

            category.Name = request.Name;

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(CacheKeys.CATEGORIES_PREFIX);
        }

        public async Task DeleteAsync(Guid categoryId)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(categoryId);

            if (category == null)           
               throw new KeyNotFoundException("Danh mục không tồn tại");

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(CacheKeys.CATEGORIES_PREFIX);
        }

        #region helper method
        private Category MappingCategory(CategoryRequestDto request)
        {
            Category category = new Category
            {
                Name = request.Name,
            };

            return category;
        }
        #endregion
    }
}
