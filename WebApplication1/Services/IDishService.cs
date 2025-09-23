using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;

namespace Food_Ordering.Services
{
    public interface IDishService
    {   
        public Task<Response<PagingResponse<MenuItemDto>>> GetAll(MenuItemQuery query);
        public Task<Response<string>> Add(MenuItemRequest request);
        public Task<Response<string>> Update(Guid id, MenuItemRequest request);
        public Task<Response<string>> Delete(Guid id);
    }
}
