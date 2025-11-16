using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;

namespace FoodOrdering.Application.Services.Interface
{
    public interface ITokenService
    {
        public Task<AuthResponse> GenerateToken(User user, HttpContext context);
        public Task<AuthResponse> GenerateRefreshToken(HttpContext context);
    }
}
