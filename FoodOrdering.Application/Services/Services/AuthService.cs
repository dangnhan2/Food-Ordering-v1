using DotNetEnv;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Email;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Serilog;


namespace FoodOrdering.Application.Services.Services
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

        public async Task ChangePasswordAsync(PasswordRequestDto request)
        {
            var result = await new PasswordValidator().ValidateAsync(request);
            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var isCorrectlyPassword = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);

            if (!isCorrectlyPassword)
                throw new InvalidDataException("Mật khẩu hiện tại không đúng");

            await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);           
        }

        public async Task ForgotPasswordAsync(ForgotPasswordRequestDto request)
        {
            Log.Information("User enter email...");
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            Log.Information("Generate otp");
            //generate opt
            var otp = await GenerateOtp(user.Id);

            Log.Information("Send email");

            SendEmail(user.Id, user.Email, otp);

            Log.Information("Send email successful");
        }

        public async Task<AuthResponse> LoginAsync(LoginRequestDto request, HttpContext context)
        {
            var result = await new LoginValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var user = await _userManager.FindByEmailAsync(request.Email);
            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (user == null || !isPasswordValid)
                throw new ArgumentException("Thông tin đăng nhập không đúng");

            var authResponse = await _tokenService.GenerateToken(user, context);

            return authResponse;
        }

        public async Task LogoutAsync(HttpContext context)
        {
            var refreshToken = context.Request.Cookies["refreshToken"];
            if (refreshToken == null)
                throw new UnauthorizedAccessException(nameof(refreshToken));

            var existToken = await _unitOfWork.RefreshToken.GetTokenByRefreshToken(refreshToken);

            if (existToken == null || existToken.ExpriedAt < DateTime.UtcNow)
                throw new UnauthorizedAccessException(nameof(refreshToken));

            var user = await _unitOfWork.User.GetByIdAsync(existToken.UserId);

            user.RefreshTokens.Clear();

            _unitOfWork.User.Update(user);
            _unitOfWork.RefreshToken.Remove(existToken);
            await _unitOfWork.SaveChangeAsync();

            context.Response.Cookies.Delete("refreshToken",
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.None,
                    Secure = false,                 
                    Path = "/"
                });
        }

        public async Task<AuthResponse> RefreshTokenAsync(HttpContext context)
        {
            var response = await _tokenService.GenerateRefreshToken(context);
            return response;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto request)
        {
            var result = await new RegisterValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var isExistUser = await _userManager.FindByEmailAsync(request.Email);

            if (isExistUser != null)
                throw new InvalidDataException("Email đã được đăng kí");

            var newUser = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                ImageUrl = _avatar,
                Email = request.Email,
                NormalizedEmail = request.Email.ToUpper(),
                PhoneNumber = null,
                PhoneNumberConfirmed = false
            };

            var response = await _userManager.CreateAsync(newUser, request.Password);

            //Create an EmailOtp object
            var otp = await GenerateOtp(newUser.Id);

            // Add user to role
            await _userManager.AddToRoleAsync(newUser, "Customer");

            SendEmail(newUser.Id, newUser.Email, otp);

            return request.Email;
        }

        public async Task ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var result = await new ResetPasswordValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

        }

        public async Task<string> VerifyEmail(EmailVerifyRequestDto request)
        {
            var existUser = await _unitOfWork.User.GetUserByEmailAsync(request.Email);

            if (existUser == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            if (existUser.EmailOtp == null)
                throw new ArgumentException("Không tồn tại mã OTP hoặc đã hết hạn");

            if (existUser.EmailOtp.Otp != request.Otp)           
                throw new ArgumentException("Mã otp không hợp lệ hoặc hết hạn");
            
                
            existUser.EmailConfirmed = true;

            _unitOfWork.EmailOtp.Remove(existUser.EmailOtp);
            await _userManager.UpdateAsync(existUser);
            await _unitOfWork.SaveChangeAsync();

            return request.Email;
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

        private void SendEmail(Guid userId, string email, string otp)
        {
            var htmlBody = $"<p>Mã xác nhận email của bạn là:</p> " +
                $" <p class=\"otp\">{otp}</p> " +
                $"<p>Mã sẽ hết hạn trong {5} phút. Không chia sẻ mã otp này cho bất kì ai</p>";

            // schedule to delete expired otp after 5 minutes
            ScheduleDeleteExpiredOpt_5mins(userId);

            BackgroundJob.Enqueue(() => _emailService.EmailSender(email, "Một email đã gửi đến email của bạn . Hãy nhập mã xác nhận", htmlBody));
        }

        private void ScheduleDeleteExpiredOpt_5mins(Guid userId)
        {
            BackgroundJob.Schedule<IBackgroundJobScheduler>(
                j => j.ScheduleDeleteExpiredOtpJob_5mins(userId),
                TimeSpan.FromMinutes(5));
        }
    }
}
