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
        public Task<Result<Carts>> AddToCartAsync(CartRequest request);
        public Task<Result<Carts>> UpdateToCartAsync(Guid id, CartRequest request);
        public Task<Result<CartDTO>> GetCartByCustomer(Guid id);
    }
}
