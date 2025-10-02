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

        public async Task<Result<PagingReponse<UserDTO>>> GetAllAsync(UserParams userParams)
        {
            var users = _unitOfWork.User.GetAll();

            if (!string.IsNullOrEmpty(userParams.FullName))            
                users = users.Where(u => u.FullName.Trim().ToLower() == userParams.FullName.Trim().ToLower());

            if (!string.IsNullOrEmpty(userParams.PhoneNumber))
                users = users.Where(u => u.PhoneNumber == u.PhoneNumber);

            if (!string.IsNullOrEmpty(userParams.Email))
                users = users.Where(u => u.Email == userParams.Email);

            var usersToDTO = await users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                ImageUrl = u.ImageUrl,
            }).Paging(userParams.Page, userParams.PageSize).AsNoTracking().ToListAsync();

            return Result<PagingReponse<UserDTO>>.Success("Lấy dữ liệu thành công",
                new PagingReponse<UserDTO>(userParams.Page, userParams.PageSize, users.Count(), usersToDTO),
                StatusCodes.Status200OK);
        } 

        public async Task<Result<User>> UploadAvatarAsync(Guid id, IFormFile file)
        {
            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                return Result<User>.Fail("Không tìm thấy user", StatusCodes.Status404NotFound);

            var url = await _cloudinaryService.UploadImage(file, folder);
            if (user.ImageUrl != _defaultAvatar)
            {
                await _cloudinaryService.DeleteImage(user.ImageUrl);         
                user.ImageUrl = url.Data;
            }
            else
            {
                user.ImageUrl = url.Data;
            }

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return Result<User>.Success("Upload thành công", user, StatusCodes.Status200OK);

        }

        public async Task<Result<User>> UploadProfileAsync(Guid id, UserRequest request)
        {
            var validator = new UserValidation();
            var result = await validator.ValidateAsync(request);
            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Result<User>.Fail(error.ErrorMessage, StatusCodes.Status400BadRequest);
                }
            }

            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                return Result<User>.Fail("Không tìm thấy user", StatusCodes.Status404NotFound);

            user.FullName = request.FullName;
            user.Email = request.Email;

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangeAsync();

            return Result<User>.Success("Cập nhật thông tin người dùng thành công", user, StatusCodes.Status200OK);

        }
    }
}
