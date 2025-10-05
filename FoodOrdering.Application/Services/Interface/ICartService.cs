using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface ICartService
    {
        public Task<ApiResponse<Carts>> AddToCartAsync(CartRequest request);
        public Task<ApiResponse<Carts>> UpdateToCartAsync(Guid id, CartRequest request);
        public Task<ApiResponse<CartDTO>> GetCartByCustomer(Guid id);
    }
}
