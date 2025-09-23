using Food_Ordering.DTOs.QueryParams;
using Food_Ordering.DTOs.Request;
using Food_Ordering.Models.Enum;
using Food_Ordering.Services.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

namespace Food_Ordering.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderRequest request)
        {
            try
            {
                var result = await _orderService.AddAsync(request);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = result.StatusCode,
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

        [HttpPatch("{id}")]
        public async Task<IActionResult> Update(Guid id, OrderStatus status)
        {
            try
            {
                var result = await _orderService.UpdateAsync(id, status);

                if (!result.Status)
                {
                    return BadRequest(new
                    {
                        Message = result.Error,
                        StatusCode = result.StatusCode
                    });
                }

                return Ok(new
                {
                    Message = result.Data,
                    StatusCode = result.StatusCode,

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

//{
//    "userId": "c604b2d9-573a-497d-a29b-9ff87f04352a",
//  "paymentMethod": "QR",
//  "items": [
//{
//        "menuItemsId": "430208d4-2228-4362-9282-d8ba0784225b",
//      "dishName": "Cà phê sữa",
//      "quantity": 2,
//      "unitPrice": 15000,
//    },
//    {
//        "menuItemsId": "aa9c3862-ef3e-49a5-a759-bf51fe618b6c",
//      "dishName": "Phở gà",
//      "quantity": 2,
//      "unitPrice": 40000,
//    }
//  ]
//}
