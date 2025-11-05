using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddToCartAsync(CartRequest request)
        {
            var user = await _unitOfWork.User.GetUserContainsCartAsync(request.UserId);
            if (user == null)           
              throw new KeyNotFoundException(nameof(user));
            
            if (user.Carts == null)
            {
                var cart = new Carts
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId
                };

                // Thêm món ăn vào cart
                foreach (var dish in request.CartItems)
                {
                    var item = new CartItems
                    {
                        Id = Guid.NewGuid(),
                        CartId = cart.Id,
                        MenuId = dish.MenuId,
                        Quantity = dish.Quantity,
                        UnitPrice = dish.UnitPrice
                    };

                    cart.CartItems.Add(item);
                }
                await _unitOfWork.Cart.AddAsync(cart);
            }
            else
            {
                var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);
                if (cart == null)
                    throw new KeyNotFoundException(nameof(cart));

                foreach (var dish in request.CartItems)
                {
                    // Find item if it already exists in cart
                    var existItem = cart.CartItems.FirstOrDefault(i => i.MenuId == dish.MenuId);
                    if (existItem != null)
                    {
                        // Increase/Decrease if quantity > 0 else quantity = 0 => remove
                        if (dish.Quantity > 0)
                            existItem.Quantity += dish.Quantity;
                        else
                            cart.CartItems.Remove(existItem);
                    }
                    else
                    {
                        // add to cart if its a new item
                        var item = new CartItems
                        {
                            Id = Guid.NewGuid(),
                            CartId = cart.Id,
                            MenuId = dish.MenuId,
                            Quantity = dish.Quantity,
                            UnitPrice = dish.UnitPrice
                        };

                        cart.CartItems.Add(item);
                    }
                }

                if (cart.CartItems.Count() > 0)
                    _unitOfWork.Cart.Update(cart);
                else
                    _unitOfWork.Cart.Remove(cart);

            }
            await _unitOfWork.SaveChangeAsync();
        }

        public async Task<CartDTO> GetCartByCustomer(Guid id)
        {
            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(id);

            if (cart == null)
                throw new KeyNotFoundException(nameof(cart));

            var cartToDTO = new CartDTO(cart, cart.CartItems.Select(ct => new CartItemDTO(ct)).ToList());
            
            return cartToDTO;
        }

    }
}
