using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;

namespace Food_Ordering.Services
{
    public interface IMenuCategoryService
    {
        public Task<Response<PagingResponse<MenuCategoryDto>>> GetAll(MenuCategoryQuery query);
        public Task<Response<string>> Add(MenuCategoryRequest request);
        public Task<Response<string>> Update(Guid id, MenuCategoryRequest request);
        public Task<Response<string>> Delete(Guid id);
    }
}
