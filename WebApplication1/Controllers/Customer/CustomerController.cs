using FoodOrdering.Application.DTOs.QueryParams;
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

        public CustomerController(IOrderService orderService)
        {
            _orderService = orderService;
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
                    result.Code,
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
    }
}
