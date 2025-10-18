using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Auth
{
    public interface IAuthService
    {
        public Task<ApiResponse<User>> RegisterAsync(RegisterRequest request);
        public Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, HttpContext context);
        public Task<ApiResponse<AuthResponse>> RefreshTokenAsync(HttpContext context);
        public Task<ApiResponse<RefreshTokens>> LogoutAsync(HttpContext context);
        public Task<ApiResponse<string>> VerifyEmail(EmailVerifyRequest request);
        public Task<ApiResponse<User>> ChangePasswordAsync(PasswordRequest request);
        public Task<ApiResponse<User>> ForgotPasswordAsync(ForgotPasswordRequest request);
        public Task<ApiResponse<User>> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
