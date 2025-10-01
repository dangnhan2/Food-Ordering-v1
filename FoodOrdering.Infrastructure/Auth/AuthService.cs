using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.Auth;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {  
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _avatar;

        public AuthService(UserManager<User> userManager, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            Env.Load();
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _avatar = Env.GetString("DEFAULT_AVATAR");
        }

        public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request, HttpContext context)
        {
            var validator = new LoginValidator();
            var result = await validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Result<AuthResponse>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if(user == null || !isPasswordValid)          
                return Result<AuthResponse>.Fail("Thông tin đăng nhập không đúng", StatusCodes.Status400BadRequest);

            var authResponse = await _tokenService.GenerateToken(user, context);

            return Result<AuthResponse>.Success("Đăng nhập thành công", authResponse, StatusCodes.Status200OK);
        }

        public async Task<Result<RefreshTokens>> LogoutAsync(HttpContext context)
        {   
            var refreshToken =  context.Request.Cookies["refresh_token"];
            var isExistToken = await _unitOfWork.RefreshToken.GetTokenByRefreshToken(refreshToken);

            if (isExistToken == null || isExistToken.ExpriedAt < DateTime.UtcNow)
                return Result<RefreshTokens>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            _unitOfWork.RefreshToken.Remove(isExistToken);
            await _unitOfWork.SaveChangeAsync();

            context.Response.Cookies.Append(
                "refresh_token",
                string.Empty,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax,
                    Secure = true,
                    Expires = DateTimeOffset.UnixEpoch
                });

            return Result<RefreshTokens>.Success("Đăng xuất thành công", isExistToken, StatusCodes.Status200OK);
        }

        public async Task<Result<AuthResponse>> RefreshTokenAsync(HttpContext context)
        {   
            var refreshToken = context.Request.Cookies["refresh_token"];
            var authResponse = await _tokenService.GenerateToken(refreshToken, context);

            return Result<AuthResponse>.Success("Refresh token successfull", authResponse, StatusCodes.Status200OK);
        }

        public async Task<Result<User>> RegisterAsync(RegisterRequest request)
        {
                var validator = new RegisterValidator();
                var result = await validator.ValidateAsync(request);
                if (!result.IsValid)
                {
                    foreach (var error in result.Errors)
                    {
                        return Result<User>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                    }
                }

                var user = await _userManager.FindByEmailAsync(request.Email);

                if (user != null)
                    return Result<User>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Email = request.Email,
                    UserName = request.FullName,
                    NormalizedEmail = request.Email.ToUpper(),
                    FullName = request.FullName,
                    ImageUrl = _avatar
                };

                var response = await _userManager.CreateAsync(newUser, request.Password);
                if (!response.Succeeded)
                    return Result<User>.Fail("Đăng kí không thành công", StatusCodes.Status400BadRequest);

                await _userManager.AddToRoleAsync(newUser, "Customer");

                return Result<User>.Success("Đăng kí thành công", newUser, StatusCodes.Status200OK);
            
        }
    }
}
