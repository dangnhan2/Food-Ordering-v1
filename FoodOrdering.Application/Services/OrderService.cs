using CloudinaryDotNet.Actions;
using Food_Ordering.Models.Enum;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Payment;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Net.payOS.Types;
using RedLockNet.SERedis;
using Serilog;
using StackExchange.Redis;
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
        private readonly RedLockFactory _redLockFactory;
        private readonly int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
        public OrderService(IUnitOfWork unitOfWork, IPaymentGateway paymentGateway, RedLockFactory redLockFactory)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _redLockFactory = redLockFactory;
        }

        public async Task<PagingReponse<OrderDTO>> GetAllAsync(OrderParams orderParams)
        {
            var orders = _unitOfWork.Order.GetAll();

            IEnumerable<OrderDTO> ordersToDTO;

            if (orderParams.Page == 0 || orderParams.PageSize == 0)
            {
                ordersToDTO = await orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDTO(o, o.OrderMenus.Select(m => new OrderMenuDTO(m)).ToList()))
                .AsNoTracking()
                .ToListAsync();
            }
            else
            {
                ordersToDTO = await orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDTO(o, o.OrderMenus.Select(m => new OrderMenuDTO(m)).ToList()))
                .Paging(orderParams.Page, orderParams.PageSize) 
                .AsNoTracking()
                .ToListAsync();
            }


            return new PagingReponse<OrderDTO>(orderParams.Page, orderParams.PageSize, orders.Count(), ordersToDTO);
        }

        public async Task<dynamic> CreateOrderByQRAsync(OrderRequest request)
        {
            Log.Information("Start to create an order with OR");

            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);

            if (cart == null)
                throw new KeyNotFoundException(nameof(cart));              

            List<ItemData> items = new List<ItemData>();

            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Address = request.Address,
                Note = request.Note,
                ExpiredAt = DateTime.UtcNow.AddMinutes(10),
                Status = OrderStatus.Pending,
                ToTalAmount = request.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                TransactionId = orderCode
            };

            // add menu to order
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


            if (request.VoucherId.HasValue)
            {   
                var resource = $"lock:voucher:{request.VoucherId.Value}";
                var expiry = TimeSpan.FromSeconds(5);

                Log.Information("Checking voucher running out of slot or not");
                using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry)) {
                    if (!redLock.IsAcquired)
                    {
                        throw new InvalidDataException("Hệ thống đang xử lý voucher này, vui lòng thử lại sau.");          
                    }

                    //create voucher redemption
                    await CreateVouherRedemption(request.VoucherId.Value, request.UserId, order.Id);
                    //update used count after create payment link
                    await UpdateVoucher(request.VoucherId.Value);
                }
            }

            Log.Information("Create payment link");

            var response = await _paymentGateway.CreatePaymentLink(request.TotalAmount, orderCode, items);
                      
            // schedule to delete cancelled order after 10 days
            ScheduleCancelledOrder_10days(order.Id);

            // schedule to update status after 10 minutes
            ScheduleExpiredOrder_10mins(order.Id);

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            Log.Information("Order created");

            return response;
        }

        public async Task CreateOrderByCODAsync(OrderRequest request)
        {
            Log.Information("Start to create an order with COD");

            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);

            if (cart == null)
                throw new KeyNotFoundException(nameof(cart));
               
            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Address = request.Address,
                Note = request.Note,
                ExpiredAt = null,
                ToTalAmount = request.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                TransactionId = orderCode,
                Status = OrderStatus.Paid
            };

            // add menu to order
            foreach (var item in cart.CartItems)
            {
                var orderItem = new OrderMenus
                {
                    OrderId = order.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice
                };
                order.OrderMenus.Add(orderItem);
            }

            if (request.VoucherId.HasValue)
            {
                var resource = $"lock:voucher:{request.VoucherId.Value}";
                var expiry = TimeSpan.FromSeconds(5);

                Log.Information("Checking voucher running out of slot or not");

                using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry))
                {
                    if (!redLock.IsAcquired)
                    {
                        throw new InvalidDataException("Hệ thống đang xử lý voucher này, vui lòng thử lại sau.");
                    }

                    //create voucher redemption
                    await CreateVouherRedemption(request.VoucherId.Value, request.UserId, order.Id);
                    //update used count after create payment link                 
                }
            }

            ScheduleCancelledOrder_10days(order.Id);

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(order);
            await _unitOfWork.SaveChangeAsync();

            Log.Information("Order created");
        }

        public async Task<PagingReponse<OrderDTO>> GetAllAsyncByCustomer(Guid id, OrderParams orderParams)
        {
            var orders = _unitOfWork.Order.GetAll();

            var ordersToDTO = await orders
                .Where(o => o.UserId == id && (o.Status == OrderStatus.Paid || o.Status == OrderStatus.Pending))
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDTO(o, o.OrderMenus
                .Select(m => new OrderMenuDTO(m)).ToList()))
                .Paging(orderParams.Page, orderParams.PageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PagingReponse<OrderDTO>(orderParams.Page, orderParams.PageSize, orders.Count(), ordersToDTO);               
        }

        private async Task CreateVouherRedemption(Guid voucherId, Guid userId, Guid orderId)
        {
            var voucherRedemption = new VoucherRedemptions
            {
                Id = Guid.NewGuid(),
                VoucherID = voucherId,
                UserID = userId,
                OrderID = orderId,
                RedeemedAt = DateTime.UtcNow
            };

            await _unitOfWork.VoucherRedemption.AddAsync(voucherRedemption);
        }

        private async Task UpdateVoucher(Guid voucherId)
        {
            var voucher = await _unitOfWork.Voucher.GetByIdAsync(voucherId);

            if (voucher == null)
                throw new KeyNotFoundException("Không tìm thấy voucher");

            voucher.UsedCount += 1;

            if (voucher.UsedCount == voucher.UsageLimit)
                voucher.IsActive = false;

            _unitOfWork.Voucher.Update(voucher);
        }

        private void ScheduleCancelledOrder_10days(Guid id)
        {
            BackgroundJob.Schedule<IBackgroundJobScheduler>(
                j => j.DeleteCancelledOrder_10days(id),
                TimeSpan.FromDays(10));
        }

        private void ScheduleExpiredOrder_10mins(Guid id)
        {
            BackgroundJob.Schedule<IBackgroundJobScheduler>(
                j => j.UpdateExpiredOrder_10mins(id),
                TimeSpan.FromMinutes(10));
        }

        
    }
}
