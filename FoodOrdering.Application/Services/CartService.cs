using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Carts>> AddToCartAsync(CartRequest request)
        {
            var cart = new Carts
            {
               Id = Guid.NewGuid(),
               UserId = request.UserId
            };

            // Thêm món ăn vào cart
            foreach(var dish in request.CartItems)
            {
                var item = new CartItems
                {
                    CartId = cart.Id,
                    MenuId = dish.MenuId,
                    Quantity = dish.Quantity,
                    UnitPrice = dish.UnitPrice
                };

                cart.CartItems.Add(item);
            };

            await _unitOfWork.Cart.AddAsync(cart);
            await _unitOfWork.SaveChangeAsync();

            return Result<Carts>.Success("Thêm vào giỏ hàng thành công", cart, StatusCodes.Status201Created);
        }

        public async Task<Result<Carts>> UpdateToCartAsync(Guid id, CartRequest request)
        {
            var cart = await _unitOfWork.Cart.GetCartWithCartItemAsync(id);

            if(cart == null)            
                return Result<Carts>.Fail("Không tìm thấy giỏ hàng", StatusCodes.Status404NotFound);

            foreach(var dish in request.CartItems)
            {   
                // Find item if it already exists in cart
                var existItem = cart.CartItems.FirstOrDefault(i => i.MenuId == dish.MenuId);
                if(existItem != null)
                {   
                    // Increase/Decrease if quantity > 0 else quantity = 0 => remove
                    if(dish.Quantity > 0)                   
                        existItem.Quantity = dish.Quantity;                   
                    else                   
                        cart.CartItems.Remove(existItem);                    
                }
                else
                {
                    var item = new CartItems
                    {
                        CartId = cart.Id,
                        MenuId = dish.MenuId,
                        Quantity = dish.Quantity,
                        UnitPrice = dish.UnitPrice
                    };

                    cart.CartItems.Add(item);
                }
            }

            _unitOfWork.Cart.Update(cart);
            await _unitOfWork.SaveChangeAsync();

            return Result<Carts>.Success("Cập nhật giỏ hàng thành công", cart, StatusCodes.Status200OK);
            
        }
    }
}
