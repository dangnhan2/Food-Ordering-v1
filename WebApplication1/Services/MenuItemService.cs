using CloudinaryDotNet;
using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Extensions.Helper;
using Food_Ordering.Models;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Services.Storage;
using Food_Ordering.Validations;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Services
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private const string folder = "Thumbnail";

        public MenuItemService(IUnitOfWork unitOfWork, ICloudinaryService cloudinary)
        {
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinary;
        }
        public async Task<Response<string>> Add(MenuItemRequest request)
        {
            var validator = new MenuItemValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage);
                }
            }

            var items = _unitOfWork.MenuItemRepo.GetAll();

            if (await items.AnyAsync(i => i.Name.Trim().ToLower() == request.Name.Trim().ToLower())) {
                return Response<string>.Fail("Menu đã tồn tại");
            }

            var response = await _cloudinaryService.UploadImage(request.File, folder);

            MenuItems item = new MenuItems
            {
                Name = request.Name,
                MenuCategoriesId = request.MenuCategoriesId,
                Description = request.Description,
                Price = request.Price,
                ImageUrl = response.Data,
                IsAvailable = request.IsAvailable,
                StockQuantity = request.StockQuantity,
                SoldQuantity = 0,
            };

            await _unitOfWork.MenuItemRepo.AddAsync(item);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Thêm menu thành công");
        }

        public async Task<Response<string>> Delete(Guid id)
        {
            var menu = await _unitOfWork.MenuItemRepo.GetByIdAsync(id);

            if(menu == null)
            {
                return Response<string>.Fail("Không tìm thấy menu");
            }

            await _cloudinaryService.DeleteImage(menu.ImageUrl);

            _unitOfWork.MenuItemRepo.Delete(menu);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Xóa menu thành công");
        }

        public async Task<Response<PagingResponse<MenuItemDto>>> GetAll(MenuItemQuery query)
        {
            var items = _unitOfWork.MenuItemRepo.GetAll();

            if (!string.IsNullOrEmpty(query.Name))
            {
                items = items.Where(i => i.Name.Trim().ToLower().Contains(query.Name.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(query.Category))
            {
                items = items.Where(i => i.MenuCategories.Name.Contains(query.Category));
            }

            if (query.IsAvailable.HasValue)
            {
                items = items.Where(i => i.IsAvailable == query.IsAvailable.Value);
            }

            var itemsToDto = items.ToPagedList(query.Page, query.PageSize).Select(i => new MenuItemDto
            {
                Id = i.Id,
                Name = i.Name,
                Category = i.MenuCategories.Name,
                Description = i.Description,
                Price = i.Price,
                ImageUrl = i.ImageUrl,
                IsAvailable = i.IsAvailable,
                SoldQuantity = i.SoldQuantity,
                StockQuantity = i.StockQuantity,
            });

            PagingResponse<MenuItemDto> response = new PagingResponse<MenuItemDto>(await itemsToDto.ToListAsync(), items.Count(), query.Page, query.PageSize);
            return Response<PagingResponse<MenuItemDto>>.Success(response);
        }

        public async Task<Response<string>> Update(Guid id, MenuItemRequest request)
        {
            var validator = new MenuItemValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage);
                }
            }

            var menu = await _unitOfWork.MenuItemRepo.GetByIdAsync(id);
            
            if (menu == null) {
                return Response<string>.Fail("Không tìm thấy menu");
            }
            
            if (request.File != null)
            {
                await _cloudinaryService.DeleteImage(menu.ImageUrl);

                var response = await _cloudinaryService.UploadImage(request.File, folder);

                menu.ImageUrl = response.Data;
            }

            menu.Name = request.Name;
            menu.MenuCategoriesId = request.MenuCategoriesId;
            menu.Description = request.Description;
            menu.Price = request.Price;
            menu.IsAvailable = request.IsAvailable;
            menu.StockQuantity = request.StockQuantity;

            _unitOfWork.MenuItemRepo.Update(menu);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Cập nhật menu thành công");
        }
    }
}
