using FoodOrdering.Application.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Payment
{
    [Route("api/[controller]")]
    [ApiController]
    public class PAYOSController : ControllerBase
    {
        private readonly IPaymentGateway _paymentGateway;

        public PAYOSController(IPaymentGateway paymentGateway)
        {
            _paymentGateway = paymentGateway;
        }

        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebHook(string url)
        {
            var result = await _paymentGateway.ConfirmWebHook(url);

            return Ok(result);
        }

        [HttpPost("callback")]
        public async Task<IActionResult> CallBack()
        {
            try
            {
                var result = await _paymentGateway.CallBack(Request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
