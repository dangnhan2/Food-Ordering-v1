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
        public Task<Result<dynamic>> CreateOrderAsync(OrderRequest request);
        public Task<Result<PagingReponse<OrderDTO>>> GetAllAsync(OrderParams orderParams);
        public Task<Result<PagingReponse<OrderDTO>>> GetAllAsyncByCustomer(Guid id, OrderParams orderParams);
    }
}
