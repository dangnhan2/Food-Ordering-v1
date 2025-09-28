using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
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

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<CategoryDTO>>> GetAllAsync()
        {
            var categories = await _unitOfWork.Category.GetAllAsync();

            var categoriesToDTO = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
            });

            return Result<IEnumerable<CategoryDTO>>.Success("Lấy dữ liệu thành công", categoriesToDTO, 200);
        }

        public async Task<Result<Categories>> AddAsync(CategoryRequest request)
        {
            var validator = new CategoryValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<Categories>.Fail($"{error.ErrorMessage}", 400);
                }
            }

            Categories categories = new Categories
            {
                Name = request.Name,
            };

            await _unitOfWork.Category.AddAsync(categories);
            await _unitOfWork.SaveChangeAsync();

            return Result<Categories>.Success($"Thêm menu {request.Name} thành công", categories, 201);
        }

        public async Task<Result<Categories>> UpdateAsync(Guid id, CategoryRequest request)
        {
            var validator = new CategoryValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<Categories>.Fail($"{error.ErrorMessage}", 400);
                }
            }
            
            var categories = await _unitOfWork.Category.GetAllAsync();
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)
            {
                return Result<Categories>.Fail("Không tìm thấy menu", 404);
            }

            if (categories.Any(c => c.Name.Equals(request.Name) && c.Id != id))
            {
                return Result<Categories>.Fail("Menu đã tồn tại", 400);
            }

            category.Name = request.Name;

            _unitOfWork.Category.Update(category);
            await _unitOfWork.SaveChangeAsync();

            return Result<Categories>.Success($"Cập nhật menu {request.Name} thành công", category, 200);
        }

        public async Task<Result<Categories>> DeleteAsync(Guid id)
        {
            var category = await _unitOfWork.Category.GetByIdAsync(id);

            if (category == null)
            {
                return Result<Categories>.Fail("Không tìm thấy menu", 404);
            }

            _unitOfWork.Category.Remove(category);
            await _unitOfWork.SaveChangeAsync();

            return Result<Categories>.Success($"Xóa menu {category.Name} thành công", category, 200);
        }
    }
}
