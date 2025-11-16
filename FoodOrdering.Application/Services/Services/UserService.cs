using DotNetEnv;
using FoodOrdering.Application.Caching;
using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using Microsoft.EntityFrameworkCore;
namespace FoodOrdering.Application.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly string _defaultAvatar;
        private const string folder = "Avatar";
        private readonly ICachingService _cachingService;

        public UserService(IUnitOfWork unitOfWork, ICloudinaryService cloudinaryService, ICachingService cachingService)
        {
            Env.Load();
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _defaultAvatar = Env.GetString("DEFAULT_AVATAR");
            _cachingService = cachingService;
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
                usersToDTO = await users
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserDTO(u))   
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                usersToDTO = await users
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserDTO(u))
                    .Paging(userParams.Page, userParams.PageSize)
                    .AsNoTracking()
                    .ToListAsync();
            }

            var response = new PagingReponse<UserDTO>(userParams.Page, userParams.PageSize, users.Count(), usersToDTO);
            return response;
        } 

        public async Task UploadProfileAsync(Guid id, UserRequest request)
        {
            var result = await new UserValidation().ValidateAsync(request);
  
            if (!result.IsValid)           
               throw new ValidationDictionaryException(result.ToDictionary());
            
            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var phoneNumbers = _unitOfWork.User.GetAll().Where(u => u.PhoneNumber == request.PhoneNumber && u.Id != id);

            if (phoneNumbers.Any())
                throw new ArgumentException("Số điện thoại đã tồn tại");

            if (request.Avatar != null)
            {
                var url = await _cloudinaryService.UploadImage(request.Avatar, folder);

                if (user.ImageUrl != _defaultAvatar)
                {
                    await _cloudinaryService.DeleteImage(user.ImageUrl);
                    user.ImageUrl = url;
                }
                else
                {
                    user.ImageUrl = url;
                }
            }
            
            user.FullName = request.FullName;
            user.PhoneNumber = request.PhoneNumber;

            _unitOfWork.User.Update(user);
            await _unitOfWork.SaveChangeAsync();
            await _cachingService.RemoveAsync(CacheKeys.UserDetail(user.Id));
        }

        public async Task<UserDTO> GetUserByIdAsync(Guid id)
        {
            var cacheKey = CacheKeys.UserDetail(id);
            var cached = await _cachingService.GetAsync<UserDTO>(cacheKey);
            if (cached != null)
                return cached;

            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var response = new UserDTO(user, "");
            await _cachingService.SetAsync(cacheKey, response, TimeSpan.FromHours(12));
            return response;
        }
    }
}
