using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sprache;

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
            var response = ApiResponse<AuthResponse>.Success("Đăng nhập thành công", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            var response = ApiResponse<string>.Success("Email đã được gửi. Hãy nhập mã để xác nhận", result, StatusCodes.Status201Created);
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken()
        {
            var result = await _authService.RefreshTokenAsync(HttpContext);
            var response = ApiResponse<AuthResponse>.Success("Refresh token successfull", result, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync(HttpContext);
            var response = ApiResponse<dynamic>.Success("Đăng xuất thành công", null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("email/verify")]
        public async Task<IActionResult> VerifyEmail([FromBody] EmailVerifyRequest request)
        {
            var result = await _authService.VerifyEmail(request);
            var response = ApiResponse<string>.Success("Xác nhận email thành công", result, StatusCodes.Status200OK);
            return Ok(response);

        }

        [HttpPost("password/change")]
        public async Task<IActionResult> ChangePassword([FromBody] PasswordRequest request)
        {
            await _authService.ChangePasswordAsync(request);
            var response = ApiResponse<dynamic>.Success("Đổi mật khẩu thành công", null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("password/forgot")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request);
            var response = ApiResponse<dynamic>.Success("Email đã được gửi. Hãy nhập mã để xác nhận", null, StatusCodes.Status200OK);
            return Ok(response);
        }

        [HttpPost("password/reset")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request);
            var response = ApiResponse<dynamic>.Success("Thiết lập mật khẩu mới thành công", null, StatusCodes.Status200OK);
            return Ok(response);
        }

    }
}
