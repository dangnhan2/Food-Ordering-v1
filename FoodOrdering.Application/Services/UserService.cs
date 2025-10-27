using DotNetEnv;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string _defaultAvatar;
        private const string folder = "Avatar";

        public UserService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService)
        {
            Env.Load();
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _defaultAvatar = Env.GetString("DEFAULT_AVATAR");
        }

        public async Task<PagingReponse<UserDTO>> GetAllAsync(UserParams userParams)
        {
            var users = _unitOfWork.User.GetAll();

            if (!string.IsNullOrEmpty(userParams.FullName))            
                users = users.Where(u => u.FullName.Trim().ToLower() == userParams.FullName.Trim().ToLower());

            if (!string.IsNullOrEmpty(userParams.PhoneNumber))
                users = users.Where(u => u.PhoneNumber == u.PhoneNumber);

            if (!string.IsNullOrEmpty(userParams.Email))
                users = users.Where(u => u.Email == userParams.Email);

            IEnumerable<UserDTO> usersToDTO;
            if (userParams.Page == 0 || userParams.PageSize == 0)
            {
                usersToDTO = await users.OrderByDescending(u => u.CreatedAt).Select(u => new UserDTO(u))   
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                usersToDTO = await users.OrderByDescending(u => u.CreatedAt).Select(u => new UserDTO(u))
                    .Paging(userParams.Page, userParams.PageSize)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return new PagingReponse<UserDTO>(userParams.Page, userParams.PageSize, users.Count(), usersToDTO);                
        } 

        public async Task UploadAvatarAsync(Guid id, IFormFile file)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException(nameof(user));

            var url = await _cloudinaryService.UploadImage(file, folder);
            if (user.ImageUrl != _defaultAvatar)
            {
                await _cloudinaryService.DeleteImage(user.ImageUrl);         
                user.ImageUrl = url;
            }
            else
            {
                user.ImageUrl = url;
            }

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangeAsync();

        }

        public async Task UploadProfileAsync(Guid id, UserRequest request)
        {
            var result = await new UserValidation().ValidateAsync(request);
  
            if (!result.IsValid)
            {
                throw new ValidationDictionaryException(result.ToDictionary());
            }

            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException(nameof(user));

            user.FullName = request.FullName;
            user.Email = request.Email;

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangeAsync();
        }
    }
}
