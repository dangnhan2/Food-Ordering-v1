using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IMenuService
    {
        public Task<PagingReponse<MenuDto>> GetAllMenusAsync(MenuParams menuParams);
        public Task<IEnumerable<MenuDto>> GetFeaturedMenusAsync();
        public Task<IEnumerable<MenuDto>> GetRelatedMenusAsync(Guid menuId);
        public Task<MenuDto> GetMenuByIdAsync(Guid menuId);
        public Task AddMenuAsync(MenuRequestDto request);
        public Task UpdateMenuAsync(Guid menuId, MenuRequestDto request);
        public Task DeleteMenuAsync(Guid menuId);
    }
}
