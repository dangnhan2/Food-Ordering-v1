using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Sprache;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FoodOrdering.Infrastructure.Services.Token
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

            string refresh = await RefreshTokenAsync(user.Id);

            var authResponse = new AuthResponse
            {
                Data = new UserDTO(user, userRole.First()),               
                AccessToken = token
            };

            context.Response.Cookies.Append(
                "refreshToken",
                refresh,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = true,
                    Expires = DateTime.UtcNow.AddMonths(3),
                    Path = "/"
                });
            return authResponse;
        }

        public async Task<AuthResponse> GenerateRefreshToken(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (refreshToken == null)
                throw new UnauthorizedAccessException("Token is invalid");

            var existRefreshToken = await _unitOfWork.RefreshToken.GetTokenByRefreshToken(refreshToken);

            if (existRefreshToken == null || existRefreshToken.ExpriedAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException("Token is invalid");

            var user = existRefreshToken.User;

            _unitOfWork.RefreshToken.Remove(existRefreshToken);          
            var authResponse = await GenerateToken(user, context);
            return authResponse;
        }

        // save refresh token to db
        private async Task<string> RefreshTokenAsync(Guid userId)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);

            if (user == null) throw new KeyNotFoundException("Không tìm thấy người dùng");
            // refreshToken
            string refresh = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString();

            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = refresh.HashToken(),
                CreatedAt = DateTime.UtcNow,
                ExpriedAt = DateTime.UtcNow.AddMonths(3)
            };

            user.RefreshTokens.Add(refreshToken);
            
            await _unitOfWork.RefreshToken.AddAsync(refreshToken);
            await _unitOfWork.SaveChangeAsync();

            return refresh;
        }
    }
}
