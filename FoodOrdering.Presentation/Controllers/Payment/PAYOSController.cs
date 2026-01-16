using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Payment;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Payment
{
    [Route("api/payment")]
    [ApiController]
    public class PAYOSController : ControllerBase
    {
        private readonly IPayOsService _payOsService;

        public PAYOSController(IPayOsService payOsService)
        {
            _payOsService = payOsService;
        }

        [HttpPost("webhook/confirm")]
        public async Task<IActionResult> ConfirmWebHook([FromBody] WebHookUrlDto request)
        {
            var result = await _payOsService.ConfirmWebHook(request);

            return Ok(result);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> CallBack()
        {

            var result = await _payOsService.CallBack(Request);

            var response = ApiResponse<dynamic>.Success("Thanh toán thành công", result, StatusCodes.Status200OK);

            return Ok(response);

            //return new JsonResult(new { }) { StatusCode = 200 };
        }


    }
}
