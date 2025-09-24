using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Models.Enum;

namespace Food_Ordering.Services.Order
{
    public interface IOrderService
    {
        public Task<Response<PagingResponse<OrderDto>>> GetAllAsync(OrderQuery query);
        public Task<Response<OrderDto>> GetByIdAsync(Guid id);
        public Task<dynamic> AddAsync(OrderRequest request);
        public Task<Response<string>> UpdateAsync(Guid id, OrderStatus status);
        public Task<Response<string>> DeleteAsync(Guid id);
    }
}
