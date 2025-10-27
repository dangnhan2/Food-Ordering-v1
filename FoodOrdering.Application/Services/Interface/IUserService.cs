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
        public Task<PagingReponse<UserDTO>> GetAllAsync(UserParams userParams);
        public Task UploadProfileAsync(Guid id, UserRequest request);
        public Task UploadAvatarAsync(Guid id, IFormFile file);
    }
}
