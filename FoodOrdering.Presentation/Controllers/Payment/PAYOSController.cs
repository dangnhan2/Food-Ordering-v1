using FoodOrdering.Application.Services.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Payment
{
    [AllowAnonymous]
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
        public async Task<IActionResult> ConfirmWebHook([FromBody] string url)
        {
            var result = await _payOsService.ConfirmWebHook(url);

            return Ok(result);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> CallBack()
        {
            //try
            //{
            //    var result = await _payOsService.CallBack(Request);
            //    return Ok(result);
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}

            return Ok("Ok");
        }
    }
}
