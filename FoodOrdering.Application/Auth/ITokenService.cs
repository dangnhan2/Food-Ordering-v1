using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Auth
{
    public interface ITokenService
    {
        public Task<AuthResponse> GenerateToken(User user, HttpContext context);
        public Task<AuthResponse> GenerateToken(string refresh, HttpContext context);
    }
}
