using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IOrderService
    {
        public Task<PaymentOrderInfo> CreateOrderByQRAsync(OrderRequestDto request);
        public Task<int> CreateOrderByCODAsync(OrderRequestDto request);
        public Task<PagingReponse<OrderDTO>> GetAllAsync(OrderParams orderParams);
        public Task<PagingReponse<OrderDTO>> GetAllAsyncByCustomer(Guid userId, OrderParams orderParams);
    }
}
