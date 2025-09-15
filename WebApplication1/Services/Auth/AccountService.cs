using DotNetEnv;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Models;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Services.Email;
using Food_Ordering.Validations;
using Microsoft.AspNetCore.Identity;
using Sprache;
using System.Net;

namespace Food_Ordering.Services.Auth
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private const string defaultAvatar = "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg";

        public AccountService(UserManager<User> userManager, IJwtService jwtService, IEmailService emailService, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Response<string>> ChangePassword(string id, PasswordRequest password)
        {
            var validator = new PasswordValidator();

            var result = validator.Validate(password);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Response<string>.Fail($"{error.ErrorMessage}");
                }
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return Response<string>.Fail("Không tìm thấy người dùng");
            }

            var response = await _userManager.ChangePasswordAsync(user, password.Password, password.NewPassword);

            if (!response.Succeeded)
            {
                foreach(var error in response.Errors) {
                    return Response<string>.Fail($"Thay đổi mật khẩu không thành công. {error.Description}");
                }
            }

            return Response<string>.Success("Thay đổi mật khẩu thành công");
        }

        public async Task<Response<LoginResponse>> Login(LoginRequest login, HttpContext context)
        {
            var validator = new LoginValidator();

            var result = validator.Validate(login);

            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    return Response<LoginResponse>.Fail(error.ErrorMessage);
                }
            }

            var user = await _userManager.FindByEmailAsync(login.Email);
            var isValidPassword = await _userManager.CheckPasswordAsync(user, login.Password);

            if (user == null || !isValidPassword)
            {
                return Response<LoginResponse>.Fail("Thông tin đăng nhập sai, vui lòng thử lại");
            }

            if (!user.EmailConfirmed)
            {
                return Response<LoginResponse>.Fail("Email chưa được xác nhận, hãy xác nhận email của bạn");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var userToDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Role = roles.First()
            };

            var tokens = await _jwtService.GenerateToken(user);

            context.Response.Cookies.Append("refreshToken", tokens.RefreshToken,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(7),
                    HttpOnly = true,
                    IsEssential = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                });

            var response = new LoginResponse
            {
                User = userToDto,
                Token = tokens.AccessToken,
            };

            return Response<LoginResponse>.Success(response);
        }

        public async Task<Response<string>> Logout(HttpContext context)
        {
            var refresh =  context.Request.Cookies["refreshToken"];

            var validRefresh = await _unitOfWork.RefreshToken.GetRefreshTokenAsync(refresh);

            if(validRefresh == null)
            {
                return Response<string>.Fail("Token is invalid");
            }

            _unitOfWork.RefreshToken.Remove(validRefresh);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Đăng xuất thành công");
        }

        public async Task<Response<LoginResponse>> Refresh(HttpContext context)
        {
            var refresh = context.Request.Cookies["refreshToken"];

            var isValidToken = await _unitOfWork.RefreshToken.GetRefreshTokenAsync(refresh) ?? throw new NullReferenceException("Token is invalid");

            var user = await _userManager.FindByIdAsync(isValidToken.UserId);

            var roles = await _userManager.GetRolesAsync(user);

            var userToDto = new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Role = roles.First(),
            };

            var tokens = await _jwtService.GenerateToken(user);

            var response = new LoginResponse
            {
                User = userToDto,
                Token = tokens.AccessToken,
            };

            context.Response.Cookies.Append("refreshToken", tokens.RefreshToken,
                   new CookieOptions
                   {
                       Expires = DateTimeOffset.UtcNow.AddDays(7),
                       HttpOnly = true,
                       IsEssential = true,
                       Secure = true,
                       SameSite = SameSiteMode.None,
                   });

            return Response<LoginResponse>.Success(response);
        }

        public async Task<Response<string>> Register(RegisterRequest register)
        {
            Env.Load();
            var validator = new RegisterValidator();

            var result = validator.Validate(register);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage);
                }
            }

            var isValidEmail = await _userManager.FindByEmailAsync(register.Email);

            if(isValidEmail != null)
            {
                return Response<string>.Fail("Email đã tồn tại, hãy sử dụng email khác");
            }

            var newUser = new User
            {
                FullName = register.FullName,
                UserName = register.FullName,
                Email = register.Email,
                ImageUrl = defaultAvatar,
                EmailConfirmed = false,
                NormalizedEmail = register.Email.ToUpper(),
            };

            var response = await _userManager.CreateAsync(newUser, register.Password);

            if (!response.Succeeded)
            {   
                foreach (var error in response.Errors)
                {
                    return Response<string>.Fail($"Đăng kí không thành công");
                }              
            }

            await _userManager.AddToRoleAsync(newUser, "Customer");

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

            token = WebUtility.UrlEncode(token);

            string subject = "Xác nhận đăng kí";

            var confirmationUrl = $"{Env.GetString("BASE_URI")}/api/EmailController/confirm-email?userId={newUser.Id}&token={token}";

            string htmlBody = $"<p>Nhấn vào link sau để xác nhận tài khoản:</p><a href='{confirmationUrl}'>Confirm</a>";

            await _emailService.SendEmailAsync(newUser.Email, subject, htmlBody);

            return Response<string>.Success("Một email đã được gửi tới email của bạn. Hãy nhấn vào đường link để xác nhận");
        }


    }
}
