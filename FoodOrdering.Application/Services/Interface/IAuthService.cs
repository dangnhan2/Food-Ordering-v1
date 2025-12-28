using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IAuthService
    {
        public Task<string> RegisterAsync(RegisterRequestDto request);
        public Task<AuthResponse> LoginAsync(LoginRequestDto request, HttpContext context);
        public Task<AuthResponse> RefreshTokenAsync(HttpContext context);
        public Task LogoutAsync(HttpContext context);
        public Task<string> VerifyEmail(EmailVerifyRequestDto request);
        public Task ChangePasswordAsync(PasswordRequestDto request);
        public Task ForgotPasswordAsync(ForgotPasswordRequestDto request);
        public Task ResetPasswordAsync(ResetPasswordRequestDto request);
    }
}
