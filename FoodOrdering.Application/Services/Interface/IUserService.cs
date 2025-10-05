using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IUserService
    {
        public Task<ApiResponse<PagingReponse<UserDTO>>> GetAllAsync(UserParams userParams);
        public Task<ApiResponse<User>> UploadProfileAsync(Guid id, UserRequest request);
        public Task<ApiResponse<User>> UploadAvatarAsync(Guid id, IFormFile file);
    }
}
