using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.Auth;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Email;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace FoodOrdering.Infrastructure.Identity
{
    public class AuthService : IAuthService
    {  
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly string _avatar;
        private readonly IEmailService _emailService;
      
        public AuthService(UserManager<User> userManager, ITokenService tokenService, IUnitOfWork unitOfWork, IEmailService emailService)
        {
            Env.Load();
            _userManager = userManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _avatar = Env.GetString("DEFAULT_AVATAR");
            _emailService = emailService;        
        }

        public async Task<ApiResponse<User>> ChangePasswordAsync(PasswordRequest request)
        {   
            var result = await new PasswordValidator().ValidateAsync(request);
            if (!result.IsValid)
                return ApiResponse<User>.Fail(result.ToDictionary(), StatusCodes.Status400BadRequest);

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
                return ApiResponse<User>.Fail("Không tìm thấy người dùng", StatusCodes.Status404NotFound);

            await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            return ApiResponse<User>.Success("Đổi mật khẩu thành công", user, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<User>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                return ApiResponse<User>.Fail("Không tìm thấy email/email chưa được đăng kí", StatusCodes.Status404NotFound);

            //generate opt
            var otp = await GenerateOtp(user.Id);

            SendEmail(user.Id, user.Email, otp);

            return ApiResponse<User>.Success("Một email đã được gửi tới email của bạn.", user, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request, HttpContext context)
        {
            var result = await new LoginValidator().ValidateAsync(request);

            if (!result.IsValid)
                return ApiResponse<AuthResponse>.Fail(result.ToDictionary(), StatusCodes.Status400BadRequest);           

            var user = await _userManager.FindByEmailAsync(request.Email);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if(user == null || !isPasswordValid)          
                return ApiResponse<AuthResponse>.Fail("Thông tin đăng nhập không đúng", StatusCodes.Status400BadRequest);
            
            var authResponse = await _tokenService.GenerateToken(user, context);

            return ApiResponse<AuthResponse>.Success("Đăng nhập thành công", authResponse, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<RefreshTokens>> LogoutAsync(HttpContext context)
        {   
            var refreshToken =  context.Request.Cookies["refresh_token"];
            
            if(refreshToken == null) 
                return ApiResponse<RefreshTokens>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

            var isExistToken = await _unitOfWork.RefreshToken.GetTokenByRefreshToken(refreshToken);

            if (isExistToken == null || isExistToken.ExpriedAt < DateTime.UtcNow)
                return ApiResponse<RefreshTokens>.Fail("Token is invalid", StatusCodes.Status401Unauthorized);

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

            return ApiResponse<RefreshTokens>.Success("Đăng xuất thành công", isExistToken, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(HttpContext context)
        {              
            var response = await _tokenService.GenerateRefreshToken(context);
            return ApiResponse<AuthResponse>.Success(response.Message, response.Data, response.StatusCode);
        }

        public async Task<ApiResponse<User>> RegisterAsync(RegisterRequest request)
        {
            var result = await new RegisterValidator().ValidateAsync(request);

            if (!result.IsValid)               
               return ApiResponse<User>.Fail(result.ToDictionary(), StatusCodes.Status400BadRequest);

            var isExistUser = await _userManager.FindByEmailAsync(request.Email);

            if (isExistUser != null)
                return ApiResponse<User>.Fail("Email đã được đăng kí", StatusCodes.Status400BadRequest);

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = Extensions.GenerateString(10),
                FullName = Extensions.GenerateString(10),
                ImageUrl = _avatar,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                PhoneNumber = null,
                PhoneNumberConfirmed = false
            };

            var response = await _userManager.CreateAsync(newUser, request.Password);

            if (!response.Succeeded)
                return ApiResponse<User>.Fail($"Đăng kí không thành công : ${response.ToString()}", StatusCodes.Status400BadRequest);

            //Create an EmailOtp object
            var otp =  await GenerateOtp(newUser.Id);          
          
            // Add user to role
            await _userManager.AddToRoleAsync(newUser, "Customer");

            SendEmail(newUser.Id, newUser.Email, otp);
           
            return ApiResponse<User>.Success("Đăng kí thành công. Một email đã được gửi tới email của bạn.", newUser, StatusCodes.Status200OK);           
        }

        public async Task<ApiResponse<User>> ResetPasswordAsync(ResetPasswordRequest request)
        {   
            var result = await new ResetPasswordValidator().ValidateAsync(request);

            if (!result.IsValid)
                return ApiResponse<User>.Fail(result.ToDictionary(), StatusCodes.Status400BadRequest); 

            var user = await _userManager.FindByEmailAsync(request.Email);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user,token, request.NewPassword);

            return ApiResponse<User>.Success("Đặt lại mật khẩu thành công", user, StatusCodes.Status200OK);

        }

        public async Task<ApiResponse<string>> VerifyEmail(EmailVerifyRequest request)
        {
            var isExistUser = await _unitOfWork.User.GetUserByEmailAsync(request.Email);

            if(isExistUser == null)
                return ApiResponse<string>.Fail("Không tìm thấy email", StatusCodes.Status404NotFound);

            if (isExistUser.EmailOtp.Otp != request.Otp)
                return ApiResponse<string>.Fail("Mã otp không hợp lệ hoặc hết hạn", StatusCodes.Status400BadRequest);

            isExistUser.EmailConfirmed = true;

            _unitOfWork.EmailOtp.Remove(isExistUser.EmailOtp);
            await _userManager.UpdateAsync(isExistUser);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<string>.Success("Xác nhận email thành công", "", StatusCodes.Status200OK);
        }

        private async Task<string> GenerateOtp(Guid id)
        {
            var otp = new Random().Next(100000, 999999).ToString();

            var emailOtp = new EmailOtp
            {
                Id = Guid.NewGuid(),
                UserId = id,
                Otp = otp
            };

            await _unitOfWork.EmailOtp.AddAsync(emailOtp);
            await _unitOfWork.SaveChangeAsync();

            return otp;

        }

        private void SendEmail(Guid userId,string email, string otp)
        {
            var htmlBody = $"<p>Mã xác nhận email của bạn là:</p> " +
                $" <p class=\"otp\">{otp}</p> " +
                $"<p>Mã sẽ hết hạn trong {5} phút. Không chia sẻ mã otp này cho bất kì ai</p>";

            // schedule to delete expired otp after 5 minutes
            BackgroundJob.Schedule<IBackgroundJobScheduler>(
                 j => j.DeleteExpiredOtp_5mins(userId),
                 TimeSpan.FromMinutes(5));

            BackgroundJob.Enqueue(() => _emailService.EmailSender(email, "Một email đã gửi đến email của bạn . Hãy nhập mã xác nhận", htmlBody));
        }
    }
}

//"userId": "fc548332-1f60-4194-923d-9eb424523f3c"
