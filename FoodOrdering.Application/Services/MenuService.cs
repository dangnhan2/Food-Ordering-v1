using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Menus>> AddAsync(MenuRequest request)
        {
            var result = await new MenuValidatior().ValidateAsync(request);
            if (!result.IsValid)            
                 return ApiResponse<Menus>.Fail(result.ToDictionary(), 400);
                      
            var menus = _unitOfWork.Menu.GetAll();

            if (menus.Any(m => m.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
                return ApiResponse<Menus>.Fail($"Menu {request.Name} đã tồn tại", StatusCodes.Status400BadRequest);

            var menu = new Menus
            {
                Name = request.Name,
                CategoriesId = request.CategoriesId,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = request.ImageUrl,
                IsAvailable = request.IsAvailble,
                StockQuantity = request.StockQuantity,
                SoldQuantity = 0
            };

            await _unitOfWork.Menu.AddAsync(menu);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Menus>.Success($"Thêm menu {menu.Name} thành công",menu, StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<Menus>> DeleteAsync(Guid id)
        {
            var menu = await _unitOfWork.Menu.GetByIdAsync(id);

            if(menu == null)            
                return ApiResponse<Menus>.Fail("Không tìm thấy menu", StatusCodes.Status404NotFound);
            
            _unitOfWork.Menu.Remove(menu);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Menus>.Success($"Xóa menu {menu.Name} thành công", menu, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<PagingReponse<MenuDTO>>> GetAllAsync(MenuParams menuParams)
        {
            var menus = _unitOfWork.Menu.GetAll();

            if (!string.IsNullOrEmpty(menuParams.Name))
                menus = menus.Where(m => m.Name.ToLower().Trim().Contains(menuParams.Name.ToLower().Trim()));

            if (!string.IsNullOrEmpty(menuParams.Category))
                menus = menus.Where(m => m.Categories.Name.ToLower().Trim().Contains(menuParams.Category.ToLower().Trim()));

            if (menuParams.IsAvailable.HasValue)
                menus = menus.Where(m => m.IsAvailable == menuParams.IsAvailable.Value);

            //sort
            if (!string.IsNullOrEmpty(menuParams.SortBy))
            {
                var sortBy = menuParams.SortBy.ToLower();
                var sortOrder = menuParams.SortOrder?.ToLower() ?? "asc";

                menus = sortBy
                switch
                {
                    "price" => sortOrder == "desc" ? menus.OrderByDescending(m => m.Price) : menus.OrderBy(m => m.Price),
                    "soldQuantity" => sortOrder == "desc" ? menus.OrderByDescending(m => m.SoldQuantity) : menus.OrderBy(m => m.SoldQuantity)
                };
            }

            var menusToDTO = await menus.Select(m => new MenuDTO
            {
                Id = m.Id,
                Category = m.Categories.Name,
                Name = m.Name,
                ImageUrl = m.ImageUrl,
                Price = m.Price,
                Description = m.Description,
                CreatedAt = m.CreatedAt,
                SoldQuantity = m.SoldQuantity,
                StockQuantity = m.StockQuantity
            }).Paging(menuParams.Page, menuParams.PageSize).AsNoTracking().ToListAsync();

            return ApiResponse<PagingReponse<MenuDTO>>.Success(
                "Lấy dữ liệu thành công",
                new PagingReponse<MenuDTO>(menuParams.Page, menuParams.PageSize, menus.Count(), menusToDTO), 
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<MenuDTO>> GetByIdAsync(Guid id)
        {
            var menu = await _unitOfWork.Menu.GetMenuWithCategoryAsync(id);
  
            if (menu == null)
               return ApiResponse<MenuDTO>.Fail("Không tìm thấy menu", StatusCodes.Status404NotFound);
           
            var menuToDto = new MenuDTO
            {
                Id = menu.Id,
                Name = menu.Name,
                Category = menu.Categories.Name,
                Description = menu.Description,
                ImageUrl = menu.ImageUrl,
                Price = menu.Price,
                CreatedAt = menu.CreatedAt,
                SoldQuantity = menu.SoldQuantity,
                StockQuantity = menu.StockQuantity
            };

            return ApiResponse<MenuDTO>.Success("Lấy dữ liệu thành công", menuToDto, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<Menus>> UpdateAsync(Guid id, MenuRequest request)
        {
            var result = await new MenuValidatior().ValidateAsync(request);
            if (!result.IsValid)
                return ApiResponse<Menus>.Fail(result.ToDictionary(), 400);

            var menu = await _unitOfWork.Menu.GetByIdAsync(id);
            var menus = _unitOfWork.Menu.GetAll();

            if (menu == null)
                return ApiResponse<Menus>.Fail("Không tìm thấy menu", StatusCodes.Status404NotFound);

            if (await menus.AnyAsync(m => m.Name.Trim().ToLower() == request.Name.Trim().ToLower() && m.Id != id))
                return ApiResponse<Menus>.Fail($"Menu {request.Name} đã tồn tại", StatusCodes.Status400BadRequest);

            menu.Name = request.Name;
            menu.ImageUrl = request.ImageUrl;
            menu.CategoriesId = request.CategoriesId;
            menu.Description = request.Description;
            menu.Price = request.Price;
            menu.IsAvailable = request.IsAvailble;
            menu.StockQuantity = request.StockQuantity;

            return ApiResponse<Menus>.Success($"Cập nhật {menu.Name} thành công", menu, StatusCodes.Status200OK);
        }

    }
}
