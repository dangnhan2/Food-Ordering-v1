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
        public Task<Result<User>> RegisterAsync(RegisterRequest request);
        public Task<Result<AuthResponse>> LoginAsync(LoginRequest request, HttpContext context);
        public Task<Result<AuthResponse>> RefreshTokenAsync(HttpContext context);
        public Task<Result<RefreshTokens>> LogoutAsync(HttpContext context);
    }
}
