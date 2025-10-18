using FoodOrdering.Application.Auth;
using FoodOrdering.Application.DTOs.Request;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrdering.Presentation.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request, HttpContext);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _authService.RefreshTokenAsync(HttpContext);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
                result.Data
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _authService.LogoutAsync(HttpContext);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerifyRequest request)
        {
            var result = await _authService.VerifyEmail(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode,
            });

        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordRequest request)
        {
            var result = await _authService.ChangePasswordAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await _authService.ForgotPasswordAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);

            return Ok(new
            {
                result.Message,
                result.StatusCode
            });
        }

    }
}
