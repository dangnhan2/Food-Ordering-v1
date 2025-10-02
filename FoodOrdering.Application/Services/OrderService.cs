using Food_Ordering.Models.Enum;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Payment;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;

        public OrderService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
        }

        public async Task<Result<PagingReponse<OrderDTO>>> GetAllAsync(OrderParams orderParams)
        {
            var orders = _unitOfWork.Order.GetAll();

            var ordersToDTO = await orders.Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                OrderStatus = o.Status,
                TotalAmount = o.ToTalAmount,
                Menus = o.OrderMenus.Select(m => new OrderMenuDTO
                {
                    Id = m.Id,
                    MenuId = m.MenuId,
                    MenuName = m.Menus.Name,
                    MenuImage = m.Menus.ImageUrl,
                    Quantity = m.Quantity,
                    UnitPrice = m.UnitPrice
                }).ToList()
            }).Paging(orderParams.Page, orderParams.PageSize).ToListAsync();

            return Result<PagingReponse<OrderDTO>>.Success("Lấy dữ liệu thành công",
                new PagingReponse<OrderDTO>(orderParams.Page, orderParams.PageSize, orders.Count(), ordersToDTO),
                StatusCodes.Status200OK);
        }

        public async Task<Result<dynamic>> CreateOrderAsync(OrderRequest request)
        {
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);

            if (cart == null)
                return Result<dynamic>.Fail("Không tìm thấy giỏ hàng", StatusCodes.Status404NotFound);

            List<ItemData> items = new List<ItemData>();

            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Address = request.Address,
                Note = request.Note,
                Status = OrderStatus.Pending,
                ToTalAmount = request.TotalAmount,
                PaymentMethod = "QRCODE",
                TransactionId = orderCode
            };

            foreach(var item in cart.CartItems)
            {
                var orderItem = new OrderMenus
                {
                    OrderId = order.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice
                };

                items.Add(new ItemData(item.Menu.Name, item.Quantity, item.UnitPrice));
                order.OrderMenus.Add(orderItem);
            }

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            var response = await _paymentGateway.CreatePaymentLink(request.TotalAmount, orderCode, items);
            
            return Result<dynamic>.Success("Tạo đơn thành công", response, StatusCodes.Status201Created);
        }

        public async Task<Result<PagingReponse<OrderDTO>>> GetAllAsyncByCustomer(Guid id, OrderParams orderParams)
        {
            var orders = _unitOfWork.Order.GetAll();

            var ordersToDTO = await orders
                .Where(o => o.UserId == id && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Pending))
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDTO
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                OrderStatus = o.Status,
                TotalAmount = o.ToTalAmount,
                Menus = o.OrderMenus.Select(m => new OrderMenuDTO
                {
                    Id = m.Id,
                    MenuId = m.MenuId,
                    MenuName = m.Menus.Name,
                    MenuImage = m.Menus.ImageUrl,
                    Quantity = m.Quantity,
                    UnitPrice = m.UnitPrice
                }).ToList()
            }).Paging(orderParams.Page, orderParams.PageSize).ToListAsync();

            return Result<PagingReponse<OrderDTO>>.Success("Lấy dữ liệu thành công",
                new PagingReponse<OrderDTO>(orderParams.Page, orderParams.PageSize, orders.Count(), ordersToDTO),
                StatusCodes.Status200OK);
        }
    }
}
