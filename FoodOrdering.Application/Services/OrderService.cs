using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task<Result<Orders>> CreateOrderAsync(OrderRequest request)
        {
            int totalAmount = 0;
            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Address = request.Address,
                Note = request.Note,
                Status = Food_Ordering.Models.Enum.OrderStatus.Pending,
                PaymentMethod = "QRCODE",
                TransactionId = orderCode
            };

            foreach(var item in request.Menus)
            {
                var orderItem = new OrderMenus
                {
                    OrderId = item.OrderId,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice
                };

                totalAmount += orderItem.SubTotal;
                order.OrderMenus.Add(orderItem);
            }

            order.ToTalAmount = totalAmount;
            await _unitOfWork.Order.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();
            
            return Result<Orders>.Success("Tạo order thành công", order, StatusCodes.Status201Created);
        }

        public async Task<Result<PagingReponse<OrderDTO>>> GetAllAsyncByCustomer(Guid id, OrderParams orderParams)
        {
            var orders = _unitOfWork.Order.GetAll();

            var ordersToDTO = await orders.Where(o => o.UserId == id).Select(o => new OrderDTO
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
