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
        public Task UploadProfileAsync(Guid userId, UserRequestDto request);
        public Task<UserDTO> GetUserByIdAsync (Guid userId);
        public Task BanUserAsync(Guid userId);
        public Task UnBanUserAsync(Guid userId);
    }
}
