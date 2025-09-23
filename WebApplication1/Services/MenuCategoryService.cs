using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Extensions.Helper;
using Food_Ordering.Models;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Validations;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Services
{
    public class MenuCategoryService : IMenuCategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MenuCategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> Add(MenuCategoryRequest request)
        {
            var validator = new MenuCategoryValidator();

            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            MenuCategories menu = new MenuCategories
            {
                Name = request.Name,
                Description = request.Description,
            };

            await _unitOfWork.MenuCategoryRepo.AddAsync(menu);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Thêm menu mới thành công", StatusCodes.Status201Created);
        }

        public async Task<Response<string>> Delete(Guid id)
        {
            var menu = await _unitOfWork.MenuCategoryRepo.GetByIdAsync(id);

            if(menu == null)
            {
                return Response<string>.Fail("Menu không tồn tại", StatusCodes.Status404NotFound);
            }

            _unitOfWork.MenuCategoryRepo.Delete(menu);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Xóa menu thành công", StatusCodes.Status200OK);
        }

        public async Task<Response<PagingResponse<MenuCategoryDto>>> GetAll(MenuCategoryQuery query)
        {
            var menus = _unitOfWork.MenuCategoryRepo.GetAll();

            var menusToDto = await menus
                .ToPagedList(query.page, query.pageSize)
                .Select(m => new MenuCategoryDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                }).ToListAsync();

            var result = new PagingResponse<MenuCategoryDto>(menusToDto, menus.Count(), query.page, query.pageSize);

            return Response<PagingResponse<MenuCategoryDto>>.Success(result, StatusCodes.Status200OK);
        }

        public async Task<Response<string>> Update(Guid id, MenuCategoryRequest request)
        {   
            var menu = await _unitOfWork.MenuCategoryRepo.GetByIdAsync(id);

            if (menu == null) {
                return Response<string>.Fail("Menu không tồn tại", StatusCodes.Status404NotFound);
            }

            var validator = new MenuCategoryValidator();

            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            menu.Name = request.Name;
            menu.Description = request.Description;

            _unitOfWork.MenuCategoryRepo.Update(menu);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Cập nhật menu thành công", StatusCodes.Status200OK);
        }
    }
}
