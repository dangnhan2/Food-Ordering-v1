using Food_Ordering.DTOs.FormQuery;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Extensions.Helper;

namespace Food_Ordering.Services
{
    public interface IUserService
    {
        public Task<Response<PagingResponse<UserDto>>> GetUsersAsync(UserQuery query);
        public Task<Response<UserDto>> GetUserAsync(string id);
        public Task<Response<string>> UserUpdateAsync(string id, UserRequest request);
        public Task<Response<string>> UploadAvatarAsync(string id, IFormFile file);

    }
}
