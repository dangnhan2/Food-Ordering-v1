using Food_Ordering.Models.Enum;
using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Services.Payment;
using FoodOrdering.Domain.Models;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;
using RedLockNet.SERedis;
using Serilog;

namespace FoodOrdering.Application.Services.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOsService _paymentGateway;
        private readonly RedLockFactory _redLockFactory;
        private readonly int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
        private readonly ICachingService _cachingService;
        private readonly INotificationSenderService _notificationSenderServer;
   
        public OrderService(
            IUnitOfWork unitOfWork, 
            IPayOsService paymentGateway, 
            RedLockFactory redLockFactory, 
            ICachingService cachingService,
            INotificationSenderService notificationSenderServer)
        {
            _unitOfWork = unitOfWork;
            _paymentGateway = paymentGateway;
            _redLockFactory = redLockFactory;
            _cachingService = cachingService;
            _notificationSenderServer = notificationSenderServer;
           
        }

        public async Task<PagingReponse<OrderDTO>> GetAllAsync(OrderParams orderParams)
        {      
            var orders = _unitOfWork.Order.GetAll();

            var ordersToDTO = orders
               .OrderByDescending(o => o.OrderDate)
               .Select(o => new OrderDTO
               {
                   Id = o.Id,
                   UserId = o.UserId,
                   OrderDate = o.OrderDate.FormatDateTimeOffset(),
                   FullName = o.Address.FullName,
                   PhoneNumber = o.Address.PhoneNumber,
                   Address = o.Address.AddressName,
                   Note = o.Note,
                   OrderStatus = o.Status,
                   TotalAmount = o.TotalAmount,
                   TransactionCode = o.TransactionId,
                   Menus = o.OrderMenus.Select(m => new OrderMenuDTO
                   {
                       Id = m.Id,
                       MenuId = m.MenuId,
                       MenuName = m.Menus.Name,
                       MenuImage = m.Menus.ImageUrl,
                       Quantity = m.Quantity,
                       SubPrice = m.UnitPrice * m.Quantity
                   }).ToList()
               })
               .AsNoTracking();
               


            if (orderParams.Page != 0 && orderParams.PageSize != 0)
            {
                ordersToDTO =  ordersToDTO.Paging(orderParams.Page, orderParams.PageSize);
            }       

                var response = new PagingReponse<OrderDTO>(orderParams.Page, orderParams.PageSize, orders.Count(), await ordersToDTO.ToListAsync());
            return response;
        }

        public async Task<dynamic> CreateOrderByQRAsync(OrderRequestDto request)
        {
            Log.Information("Start to create an order with OR");
       
            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);
            if (cart == null)
                throw new KeyNotFoundException("Giỏ hàng trống / không tồn tại");

            int totalAmount = GetSubAmount(cart.CartItems);

            var newOrder = MappingOrder(request, totalAmount, "QR");

            // add menu to order
            MappingMenuToOrder(cart.CartItems, newOrder);
            // add to list for payment
            var listItems = AddItemsPayment(cart.CartItems);

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
                        throw new KeyNotFoundException("Mã giảm giá không tồn tại");

                    int discountValue = totalAmount * voucher.DiscountValue / 100;

                    if (discountValue > voucher.MaxDiscount)
                        discountValue = voucher.MaxDiscount;

                    totalAmount = totalAmount - discountValue;

                    voucher.UsedCount += 1;

                    if (voucher.UsedCount >= voucher.UsageLimit)
                        voucher.IsActive = false;

                    _unitOfWork.Voucher.Update(voucher);
                    //create voucher redemption
                    await CreateVouherRedemption(request.VoucherId.Value, request.UserId, newOrder.Id);
                   
                }
            }

            Log.Information("Create payment link");
            var response = await _paymentGateway.CreatePaymentLink(totalAmount, orderCode, listItems);

            Log.Information("Created!!");

            // schedule to update status after 10 minutes
            ScheduleExpiredOrder_10mins(newOrder.Id);

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(newOrder);
            await _unitOfWork.SaveChangeAsync();

            Log.Information("Order created");

            return response;
        }

        public async Task<int> CreateOrderByCODAsync(OrderRequestDto request)
        {
            Log.Information("Start to create an order with COD");

            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);

            if (cart == null)
                throw new KeyNotFoundException("Giỏ hàng trống / không tồn tại");

            var totalAmount = GetSubAmount(cart.CartItems);
            var newOrder = MappingOrder(request, totalAmount, "COD");

            // add menu to order
            MappingMenuToOrder(cart.CartItems, newOrder);
            await UpdateSoldQuantity(cart.CartItems);

            if (request.VoucherId.HasValue)
            {
                var resource = $"lock:voucher:{request.VoucherId.Value}";
                var expiry = TimeSpan.FromSeconds(5);

                Log.Information("Checking voucher running out of slot");

                using (var redLock = await _redLockFactory.CreateLockAsync(resource, expiry))
                {
                    if (!redLock.IsAcquired)                    
                       throw new InvalidDataException("Hệ thống đang xử lý voucher này, vui lòng thử lại sau.");                   

                    var voucher = await _unitOfWork.Voucher.GetByIdAsync(request.VoucherId.Value);
                    if (voucher == null)
                        throw new KeyNotFoundException("Mã giảm giá không tồn tại");

                    int discountValue = totalAmount * voucher.DiscountValue / 100;

                    if (discountValue > voucher.MaxDiscount)
                        discountValue = voucher.MaxDiscount;

                    totalAmount = totalAmount - discountValue;

                    voucher.UsedCount += 1;

                    if (voucher.UsedCount >= voucher.UsageLimit)
                        voucher.IsActive = false;

                    // update voucher after increase voucher used count
                    _unitOfWork.Voucher.Update(voucher);
                    //create voucher redemption
                    await CreateVouherRedemption(request.VoucherId.Value, request.UserId, newOrder.Id);
                }
            }

            _unitOfWork.Cart.Remove(cart);
            await _unitOfWork.Order.AddAsync(newOrder);
            await _unitOfWork.SaveChangeAsync();

            Log.Information("Send notification to admin");
            await _notificationSenderServer.NotifyAdminAsync(newOrder.TransactionId, newOrder.TotalAmount);

            Log.Information("Order created");
            return newOrder.TransactionId;
        }

        public async Task<PagingReponse<OrderDTO>> GetAllAsyncByCustomer(Guid id, OrderParams orderParams)
        {
            var orders = _unitOfWork.Order.GetAll().Where(o => o.UserId == id);

            var ordersToDTO = orders
               .OrderByDescending(o => o.OrderDate)
               .Select(o => new OrderDTO
               {
                   Id = o.Id,
                   UserId = o.UserId,
                   OrderDate = o.OrderDate.FormatDateTimeOffset(),
                   FullName = o.Address.FullName,
                   PhoneNumber = o.Address.PhoneNumber,
                   Address = o.Address.AddressName,
                   OrderStatus = o.Status,
                   TotalAmount = o.TotalAmount,
                   TransactionCode = o.TransactionId,
                   Menus = o.OrderMenus.Select(m => new OrderMenuDTO
                   {
                       Id = m.Id,
                       MenuId = m.MenuId,
                       MenuName = m.Menus.Name,
                       MenuImage = m.Menus.ImageUrl,
                       Quantity = m.Quantity,
                       SubPrice = m.UnitPrice * m.Quantity
                   }).ToList()
               })
               .AsNoTracking();

            if (orderParams.Page != 0 && orderParams.PageSize != 0)
            {
                ordersToDTO = ordersToDTO.Paging(orderParams.Page, orderParams.PageSize);
            }

            return new PagingReponse<OrderDTO>(orderParams.Page, orderParams.PageSize, orders.Count(), await ordersToDTO.ToListAsync());               
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

        private Order MappingOrder(OrderRequestDto request,int total, string type)
        {
            var newOrder = new Order
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                AddressId = request.AddressId,
                Note = request.Note,
                TotalAmount = total,
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

        private void ScheduleExpiredOrder_10mins(Guid id)
        {
            BackgroundJob.Schedule<IBackgroundJobScheduler>(
                j => j.ScheduleUpdateExpiredOrderJob_10mins(id),
                TimeSpan.FromMinutes(10));
        }
        
        private async Task UpdateSoldQuantity(ICollection<CartItem> cartItems)
        {
            foreach (var item in cartItems)
            {
                var menu = await _unitOfWork.Menu.GetByIdAsync(item.MenuId);

                if (menu == null) continue;
                menu.SoldQuantity = menu.SoldQuantity + item.Quantity;
                await _cachingService.RemoveAsync(CacheKeys.MenuDetail(menu.Id));
            }
        }

        private List<ItemData> AddItemsPayment(ICollection<CartItem> cartItems)
        {
            List<ItemData> items = new List<ItemData>();

            foreach(var item in cartItems)
            {
                items.Add(new ItemData(item.Menu.Name, item.Quantity, item.UnitPrice));
            }

            return items;
        }

        private int GetSubAmount(ICollection<CartItem> items)
        {
            int TAX_RATE = 8;
            int subTotal = 0;
            foreach (var item in items)
            {
                subTotal += item.Quantity * item.UnitPrice;
            }

            subTotal = subTotal + (subTotal * TAX_RATE) / 100;
            return subTotal;
        }

        private void MappingMenuToOrder(ICollection<CartItem> cartItems, Order order)
        {
            foreach (var item in cartItems)
            {
                var orderItem = new OrderMenus
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MenuId = item.MenuId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice
                };

                order.OrderMenus.Add(orderItem);
            }
        }
    }
}
