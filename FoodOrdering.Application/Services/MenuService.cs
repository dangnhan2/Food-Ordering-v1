using CloudinaryDotNet.Actions;
using FoodOrdering.Application.Caching;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;

namespace FoodOrdering.Application.Services
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private const string folder = "Thumbnail";
        private readonly ICloudinaryService _cloudinaryService;

        public MenuService(IUnitOfWork unitOfWork, ICacheService cacheService, ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task AddAsync(MenuRequest request)
        {                    
            var result = await new MenuValidatior().ValidateAsync(request);

            if (!result.IsValid)
              throw new ValidationDictionaryException(result.ToDictionary());

            var menus = _unitOfWork.Menu.GetAll();

            if (menus.Any(m => m.Name.Trim().ToLower() == request.Name.Trim().ToLower()))
                throw new DuplicateNameException($"Menu {request.Name} đã tồn tại");

            var menu = new Menus
            {
                Name = request.Name,
                CategoriesId = request.CategoriesId,
                Description = request.Description,
                Price = request.Price,
                IsAvailable = request.IsAvailble,
                SoldQuantity = 0
            };

            if (request.Thumbnail != null)
            {
                var url = await _cloudinaryService.UploadImage(request.Thumbnail, folder);
                menu.ImageUrl = url;
            }
                
            await _unitOfWork.Menu.AddAsync(menu);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var menu = await _unitOfWork.Menu.GetByIdAsync(id);

            if(menu == null)            
              throw new KeyNotFoundException(nameof(menu));
            
            _unitOfWork.Menu.Remove(menu);
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<PagingReponse<MenuDto>> GetAllAsync(MenuParams menuParams)
        {
            //string cacheKey = $"menu_page_{menuParams.Page}_size_{menuParams.PageSize}";
            //var cached = await _cacheService.GetAsync<IEnumerable<MenuDto>>(cacheKey);

            //if (cached != null)
            //    return new PagingReponse<MenuDto>(menuParams.Page, menuParams.PageSize, cached.Count(), cached);
                                     
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
                var sortOrder = menuParams.SortOrder?.ToLower();

                menus = sortBy
                switch
                {
                    "price" => sortOrder == "desc" ? menus.OrderByDescending(m => m.Price) : menus.OrderBy(m => m.Price),
                    "soldQuantity" => sortOrder == "desc" ? menus.OrderByDescending(m => m.SoldQuantity) : menus.OrderBy(m => m.SoldQuantity),
                    _ => menus.OrderByDescending(m => m.CreatedAt)
                };
            }

            IEnumerable<MenuDto> menusToDTO;
            if (menuParams.Page == 0 && menuParams.PageSize == 0)
            {
                menusToDTO = await menus
                .Include(m => m.Categories)
                .Select(m => new MenuDto(m))
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            }
            else
            {
                menusToDTO = await menus
                  .Include(m => m.Categories)
                  .Select(m => new MenuDto(m))
                  .Paging(menuParams.Page, menuParams.PageSize)
                  .AsNoTrackingWithIdentityResolution()
                  .ToListAsync();
            }
           
            //await _cacheService.SetAsync(cacheKey, menusToDTO, TimeSpan.FromHours(12));

            return new PagingReponse<MenuDto>(menuParams.Page, menuParams.PageSize, menus.Count(), menusToDTO);
        }

        public async Task<MenuDto> GetByIdAsync(Guid id)
        {
            string cacheKey = $"menu:{id}";
            var cacheMenu = await _cacheService.GetAsync<MenuDto>(cacheKey);

            if (cacheMenu != null)
                return cacheMenu;

            var menu = await _unitOfWork.Menu.GetMenuWithCategoryAsync(id);

            if (menu == null)
                throw new KeyNotFoundException(nameof(menu));
                
            await _cacheService.SetAsync(cacheKey, new MenuDto(menu), TimeSpan.FromMinutes(10));

            return new MenuDto(menu);
        }

        public async Task UpdateAsync(Guid id, MenuRequest request)
        {          
            string cacheKey = $"menu:{id}";
            var result = await new MenuValidatior().ValidateAsync(request);
            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var menu = await _unitOfWork.Menu.GetByIdAsync(id);
            var menus = _unitOfWork.Menu.GetAll();

            if (menu == null)
               throw new KeyNotFoundException(nameof(menus));

            if (await menus.AnyAsync(m => m.Name.Trim().ToLower() == request.Name.Trim().ToLower() && m.Id != id))
                throw new DuplicateNameException($"Menu {request.Name} đã tồn tại");
            
            menu.Name = request.Name;
            menu.CategoriesId = request.CategoriesId;
            menu.Description = request.Description;
            menu.Price = request.Price;
            menu.IsAvailable = request.IsAvailble;

            if (request.Thumbnail != null)
            {
                var url = await _cloudinaryService.UploadImage(request.Thumbnail, folder);
                await _cloudinaryService.DeleteImage(menu.ImageUrl);
                menu.ImageUrl = url;
            }          

            _unitOfWork.Menu.Update(menu);
            await _unitOfWork.SaveChangeAsync();
            await _cacheService.RemoveAsync(cacheKey);
        }

    }
}
