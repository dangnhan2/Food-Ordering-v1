using Food_Ordering.Models;
using Food_Ordering.Services.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Food_Ordering.Controllers.Email
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public EmailController(UserManager<User> userManager) { 
           _userManager = userManager;
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> SendEmailConfirm(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId); 

            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
            {   
                foreach (var error in result.Errors)
                {
                    return BadRequest(new
                    {
                        Message = error.Description,
                        StatusCode = StatusCodes.Status400BadRequest
                    });
                }
                
            }

            return Ok(new
            {
                Message = "Xác nhận email thành công",
                StatusCode = StatusCodes.Status200OK
            });
        }
    }
}
