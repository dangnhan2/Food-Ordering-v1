using CloudinaryDotNet.Actions;
using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Extensions.Helper;
using Food_Ordering.Models;
using Food_Ordering.Models.Enum;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Services.Payment;
using Food_Ordering.Validations;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;

namespace Food_Ordering.Services.Order
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPayOSService _payOSService;
        public OrderService(IUnitOfWork unitOfWork, IPayOSService payOSService)
        {
            _unitOfWork = unitOfWork;
            _payOSService = payOSService;
        }

        public async Task<dynamic> AddAsync(OrderRequest request)
        {
            var validator = new OrderValidator();
            var result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
            int total = 0;
            List<ItemData> items = new List<ItemData>();

            Orders order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                Status = OrderStatus.Pending,
                PaymentMethod = request.PaymentMethod,
                TransactionId = orderCode
            };

            foreach(var item in request.Items)
            {
                var orderItems = new OrderItems
                {
                    Id = Guid.NewGuid(),
                    OrderId = order.Id,
                    MenuItemsId = item.MenuItemsId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.Quantity * item.UnitPrice,

                };

                total += orderItems.SubTotal;
                items.Add(new ItemData(item.DishName, item.Quantity, item.UnitPrice));
                order.Items.Add(orderItems);
            }

            order.ToTalAmount = total;

            _unitOfWork.OrderRepo.Add(order);
            await _unitOfWork.SaveAsync();

            var paymentLink =  await _payOSService.CreatePaymentLink(orderCode, total, items);
            return Response<dynamic>.Success(paymentLink, StatusCodes.Status201Created);
        }

        public async Task<Response<string>> DeleteAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepo.GetByIdAsync(id);

            if(order == null)
            {
                return Response<string>.Fail("Không tìm thấy order", StatusCodes.Status404NotFound);
            }

            _unitOfWork.OrderRepo.Delete(order);
            await _unitOfWork.SaveAsync();
            return Response<string>.Success("Xóa order thành công", StatusCodes.Status200OK);
        }

        public async Task<Response<PagingResponse<OrderDto>>> GetAllAsync(OrderQuery query)
        {
            var orders = _unitOfWork.OrderRepo.GetAll();

            var ordersToDto = orders
                .ToPagedList(query.Page, query.PageSize)
                .OrderBy(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    ToTalAmount = o.ToTalAmount,
                    PaymentMethod = o.PaymentMethod,
                });

            PagingResponse<OrderDto> response = new PagingResponse<OrderDto>(await ordersToDto.ToListAsync(), orders.Count(), query.Page, query.PageSize);

            return Response<PagingResponse<OrderDto>>.Success(response, StatusCodes.Status200OK);
        }

        public async Task<Response<OrderDto>> GetByIdAsync(Guid id)
        {
            var order = await _unitOfWork.OrderRepo.GetByIdAsync(id);

            if(order == null)
            {
                return Response<OrderDto>.Fail("Không tìm thấy order", StatusCodes.Status404NotFound);
            }

            var orderToDto = new OrderDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                Status = order.Status,
                ToTalAmount = order.ToTalAmount,
                PaymentMethod = order.PaymentMethod,
                OrderItems = order.Items.Select(i => new OrderItemDto
                {
                    MenuItemsId = i.MenuItemsId,
                    ItemName = i.MenuItems.Name,
                    ImageUrl = i.MenuItems.ImageUrl,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    SubTotal = i.SubTotal
                }).ToList(),
            };

            return Response<OrderDto>.Success(orderToDto, StatusCodes.Status200OK);
        }

        public async Task<Response<string>> UpdateAsync(Guid id, OrderStatus status)
        {
            var order = await _unitOfWork.OrderRepo.GetByIdAsync(id);

            if(order == null)
            {
                return Response<string>.Fail("Không tìm thấy order", StatusCodes.Status404NotFound);
            }

            order.Status = status;

            _unitOfWork.OrderRepo.Update(order);    
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Cập nhật order thành công", StatusCodes.Status200OK);
        }
    }
}
