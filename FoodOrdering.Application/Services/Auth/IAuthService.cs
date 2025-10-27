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

namespace FoodOrdering.Application.Services.Auth
{
    public interface IAuthService
    {
        public Task<string> RegisterAsync(RegisterRequest request);
        public Task<AuthResponse> LoginAsync(LoginRequest request, HttpContext context);
        public Task<AuthResponse> RefreshTokenAsync(HttpContext context);
        public Task LogoutAsync(HttpContext context);
        public Task<string> VerifyEmail(EmailVerifyRequest request);
        public Task ChangePasswordAsync(PasswordRequest request);
        public Task ForgotPasswordAsync(ForgotPasswordRequest request);
        public Task ResetPasswordAsync(ResetPasswordRequest request);
    }
}
