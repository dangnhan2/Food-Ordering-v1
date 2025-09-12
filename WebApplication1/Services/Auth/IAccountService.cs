using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;

namespace Food_Ordering.Services.Auth
{
    public interface IAccountService
    {
        public Task<Response<LoginResponse>> Login(LoginRequest login, HttpContext context);
        public Task<Response<string>> Register(RegisterRequest register);
        public Task<Response<LoginResponse>> Refresh(HttpContext context);
        public Task<Response<string>> ChangePassword(string id, PasswordRequest password);
        public Task<Response<string>> Logout(HttpContext context);
    }
}
