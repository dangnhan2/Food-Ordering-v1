using Food_Ordering.DTOs.FormQuery;
using Food_Ordering.DTOs.Request;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Extensions.Helper;
using Food_Ordering.Models;
using Food_Ordering.Repositories.UnitOfWork;
using Food_Ordering.Services.Storage;
using Food_Ordering.Validations;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private const string folder = "Avatar";
        public UserService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService) { 
          _unitOfWork = unitOfWork;
          _cloudinaryService = cloudinaryService;
        }

        public async Task<Response<UserDto>> GetUserAsync(string id)
        {
            var user = await _unitOfWork.UserRepo.GetUserAsync(id);

            if(user == null)
            {
                return Response<UserDto>.Fail("Người dùng không tồn tại");
            }

            var userToDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                UserName = user.UserName,
                ImageUrl = user.ImageUrl,
                Phone = user.PhoneNumber
            };

            return Response<UserDto>.Success(userToDto);
        }

        public async Task<Response<PagingResponse<UserDto>>> GetUsersAsync(UserQuery query)
        {
            var users = _unitOfWork.UserRepo.GetUsers();

            if (!string.IsNullOrEmpty(query.email)) { 
               users = users.Where(u => u.Email.Contains(query.email));
            }

            if (!string.IsNullOrEmpty(query.fullName)) {
                users = users.Where(u => u.FullName.Trim().ToLower().Contains(query.fullName.Trim().ToLower()));
            }

            if (!string.IsNullOrEmpty(query.phone))
            {
                users = users.Where(u => u.PhoneNumber.Contains(query.phone));
            }

            var usersToDto = await users
                .OrderByDescending(u => u.CreatedAt)
                .ToPagedList(query.page, query.pageSize)
                .Select(u => new UserDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                UserName = u.UserName,
                ImageUrl = u.ImageUrl,
                Phone = u.PhoneNumber
            }).ToListAsync();

            return Response<PagingResponse<UserDto>>.Success(new PagingResponse<UserDto>(usersToDto, users.Count(), query.page, query.pageSize));
        }

        public async Task<Response<string>> UploadAvatarAsync(string id, IFormFile file)
        {   
            var user = await _unitOfWork.UserRepo.GetUserAsync(id);

            if(user == null)
            {
                Response<string>.Fail("Không tìm thấy người dùng");
            }

            var result = await _cloudinaryService.UploadImage(file, folder);

            if (user.ImageUrl == "https://res.cloudinary.com/dtihvekmn/image/upload/v1751645852/istockphoto-1337144146-612x612_llpkam.jpg")
            {
                user.ImageUrl = result.Data;
            }
            else
            {
                await _cloudinaryService.DeleteImage(user.ImageUrl);
                user.ImageUrl = result.Data;
            }

            return Response<string>.Success("Thay đổi hình đại diện thành công");
        }

        public async Task<Response<string>> UserUpdateAsync(string id, UserRequest request)
        {
            var validator = new UserValidator();
            var result = validator.Validate(request);

            if (!result.IsValid)
            {
                foreach(var error in result.Errors)
                {
                    return Response<string>.Fail(error.ErrorMessage);
                }
            }

            var users = _unitOfWork.UserRepo.GetUsers();

            if(await users.AnyAsync(u => u.PhoneNumber == request.Phone))
            {
                return Response<string>.Fail("Số điện thoại đã được đăng ký");
            }

            var user = await _unitOfWork.UserRepo.GetUserAsync(id);

            if (user == null) {
                return Response<string>.Fail("Không tìm thấy người dùng");
            }

            user.UserName = request.UserName;
            user.FullName = request.FullName;
            user.PhoneNumber = request.Phone;
            
            _unitOfWork.UserRepo.UpdateUser(user);
            await _unitOfWork.SaveAsync();

            return Response<string>.Success("Cập nhật thông tin thành công");
            
        }
    }
}
