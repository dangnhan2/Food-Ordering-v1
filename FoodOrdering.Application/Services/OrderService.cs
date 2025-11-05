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
using Net.payOS.Types;
using RedLockNet.SERedis;
using Serilog;

namespace FoodOrdering.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentGateway _paymentGateway;
        private readonly RedLockFactory _redLockFactory;
        private readonly int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
        private const int TAX_RATE = 8;
        private int _temporaryAmount = 0;
        private int _totalAmount = 0;
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
                .Include(o => o.Address)
                .Select(o => new OrderDTO(o, o.OrderMenus.Select(m => new OrderMenuDTO(m)).ToList()))
                .AsNoTrackingWithIdentityResolution()
                .ToListAsync();
            }
            else
            {
                ordersToDTO = await orders
                .OrderByDescending(o => o.OrderDate)
                .Include(o => o.Address)
                .Select(o => new OrderDTO(o, o.OrderMenus.Select(m => new OrderMenuDTO(m)).ToList()))
                .Paging(orderParams.Page, orderParams.PageSize)
                .AsNoTrackingWithIdentityResolution()
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

            var newOrder = MappingOrder(request, "QR");

            // add menu to order
            foreach(var item in cart.CartItems)
            {
                var orderItem = MappingOrderMenus(item, newOrder.Id);
                // get total of each item 
                _temporaryAmount += orderItem.SubTotal;
                items.Add(new ItemData(item.Menu.Name, item.Quantity, item.UnitPrice));
                newOrder.OrderMenus.Add(orderItem);
            }
             _totalAmount = _temporaryAmount + (_temporaryAmount * TAX_RATE / 100);

            if (request.VoucherId.HasValue)
            {   
                var resource = $"lock:voucher:{request.VoucherId.Value}";
                var expiry = TimeSpan.FromSeconds(5);

                Log.Information("Checking voucher running out of slot or not");
                using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry)) {
                    if (!redLock.IsAcquired)                   
                       throw new InvalidDataException("Hệ thống đang xử lý voucher này, vui lòng thử lại sau.");          
                    
                    var voucher = await _unitOfWork.Voucher.GetByIdAsync(request.VoucherId.Value);
                    if (voucher == null)
                        throw new KeyNotFoundException(nameof(voucher));

                    voucher.UsedCount++;

                    int discountValue = _totalAmount * 5 / 100;

                    if (discountValue > voucher.MaxDiscount)
                        _totalAmount = _totalAmount - discountValue;

                    //create voucher redemption
                    await CreateVouherRedemption(request.VoucherId.Value, request.UserId, newOrder.Id);
                    //update used count after create payment link
                    await UpdateVoucher(request.VoucherId.Value);
                   
                }
            }

            Log.Information("Create payment link");          
            var response = await _paymentGateway.CreatePaymentLink(_totalAmount, orderCode, items);
            Log.Information("Created!!");
            
            // schedule to delete cancelled order after 10 days
            ScheduleCancelledOrder_10days(newOrder.Id);
            // schedule to update status after 10 minutes
            ScheduleExpiredOrder_10mins(newOrder.Id);

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(newOrder);
            await _unitOfWork.SaveChangeAsync();

            Log.Information("Order created");

            return response;
        }

        public async Task<int> CreateOrderByCODAsync(OrderRequest request)
        {
            Log.Information("Start to create an order with COD");

            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);

            if (cart == null)
                throw new KeyNotFoundException(nameof(cart));

            var newOrder = MappingOrder(request, "COD");

            // add menu to order
            foreach (var item in cart.CartItems)
            {
                var orderItem = MappingOrderMenus(item, newOrder.Id);
                _temporaryAmount += orderItem.SubTotal;
                newOrder.OrderMenus.Add(orderItem);
            }

            _totalAmount = _temporaryAmount + (_temporaryAmount * TAX_RATE / 100);

            if (request.VoucherId.HasValue)
            {
                var resource = $"lock:voucher:{request.VoucherId.Value}";
                var expiry = TimeSpan.FromSeconds(5);

                Log.Information("Checking voucher running out of slot or not");

                using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry))
                {
                    if (!redLock.IsAcquired)                    
                       throw new InvalidDataException("Hệ thống đang xử lý voucher này, vui lòng thử lại sau.");
                    

                    var voucher = await _unitOfWork.Voucher.GetByIdAsync(request.VoucherId.Value);
                    if (voucher == null)
                        throw new KeyNotFoundException(nameof(voucher));

                    voucher.UsedCount++;

                    int discountValue = _totalAmount * 5 / 100;

                    if (discountValue > voucher.MaxDiscount)
                        _totalAmount = _totalAmount - discountValue;
                    //create voucher redemption
                    await CreateVouherRedemption(request.VoucherId.Value, request.UserId, newOrder.Id);
                    //update used count after create payment link
                    await UpdateVoucher(request.VoucherId.Value);
                }
            }

            ScheduleCancelledOrder_10days(newOrder.Id);

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(newOrder);
            await _unitOfWork.SaveChangeAsync();

            Log.Information("Order created");
            return newOrder.TransactionId;
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

        private Orders MappingOrder(OrderRequest request, string type)
        {
            var newOrder = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                AddressId = request.AddressId,
                Note = request.Note,
                TotalAmount = request.TotalAmount,
                PaymentMethod = request.PaymentMethod,
                TransactionId = orderCode
            };

            if (type == "QR")
            {
                newOrder.ExpiredAt = DateTime.UtcNow.AddMinutes(10);
                newOrder.Status = OrderStatus.Pending;
            }else if (type == "COD")
            {
                newOrder.ExpiredAt = null;
                newOrder.Status = OrderStatus.Paid;
            }
            return newOrder;
        }

        private OrderMenus MappingOrderMenus(CartItems item, Guid id)
        {
            var orderItem = new OrderMenus
            {
                OrderId = id,
                MenuId = item.MenuId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                SubTotal = item.Quantity * item.UnitPrice
            };

            return orderItem;
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
