using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Customer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        public readonly IOrderService _orderService;
        public readonly IVoucherService _voucherService;
        public CustomerController(IOrderService orderService, IVoucherService voucherService)
        {
            _orderService = orderService;
            _voucherService = voucherService;
        }

        [HttpGet("order/{id}")]
        public async Task<IActionResult> GetAllOrderByCustomer(Guid id, [FromQuery] OrderParams orderParams)
        {
            try
            {
                var result = await _orderService.GetAllAsyncByCustomer(id, orderParams);

                return Ok(new
                {
                    result.Message,
                    result.StatusCode,
                    result.Data
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.InnerException.Message ?? ex.Message,
                    StatusCode = StatusCodes.Status400BadRequest
                });
            }
        }

        [HttpGet("voucher-of-customer")]
        public async Task<IActionResult> GetAllVoucherByCustomer()
        {
            var result = await _voucherService.GetAllByCustomerAsync();

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPost("validate-voucher")]
        public async Task<IActionResult> ValidateVoucher(ValidateVoucherRequest request)
        {
           var result = await _voucherService.ValidateVoucherAsync(request);

            if (!result.IsSuccess)
            {
                return BadRequest(new
                {
                    result.Message,
                    result.StatusCode,
                });
            }

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }
    }
}
