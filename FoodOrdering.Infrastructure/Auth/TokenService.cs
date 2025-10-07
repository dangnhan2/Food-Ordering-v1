using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.Auth;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sprache;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Identity
{
    public class TokenService : ITokenService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly string _issuer;
        private readonly string _audience;
        public TokenService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            Env.Load();
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("SECRET_KEY")));
            _issuer = Env.GetString("ISSUER");
            _audience = Env.GetString("AUDIENCE");
        }
        public async Task<AuthResponse> GenerateToken(User user, HttpContext context)
        {
            var credentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);  
            
            // add claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var userRole = await _userManager.GetRolesAsync(user);
            claims.AddRange(userRole.Select(role => new Claim(ClaimTypes.Role, role)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = credentials,
                Issuer = _issuer,
                Audience = _audience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwt);

            // refreshToken
            string refresh = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var refreshToken = new RefreshTokens
            {
                UserId = user.Id,
                Token = refresh.HashToken(),
                CreatedAt = DateTime.UtcNow,
                ExpriedAt = DateTime.UtcNow.AddMonths(3)
            };

            var authResponse = new AuthResponse
            {
                Data = new UserDTO
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    ImageUrl = user.ImageUrl,
                    Role = userRole.First()
                },
                AccessToken = token
            };

            await _unitOfWork.RefreshToken.AddAsync(refreshToken);
            await _unitOfWork.SaveChangeAsync();

            context.Response.Cookies.Append(
                "refresh_token",
                refresh,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMonths(3)
                });
            return authResponse;
        }

        public async Task<ApiResponse<AuthResponse>> GenerateRefreshToken(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refresh_token"];
            if (refreshToken == null)
                return ApiResponse<AuthResponse>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            var existRefreshToken = await _unitOfWork.RefreshToken.GetTokenByRefreshToken(refreshToken);

            if (existRefreshToken == null || existRefreshToken.ExpriedAt < DateTime.UtcNow)
                return ApiResponse<AuthResponse>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            var user = existRefreshToken.User;

            _unitOfWork.RefreshToken.Remove(existRefreshToken);          
            var authResponse = await GenerateToken(user, context);
            return ApiResponse<AuthResponse>.Success("Refresh token successfull", authResponse, StatusCodes.Status200OK);
        }
    }
}
