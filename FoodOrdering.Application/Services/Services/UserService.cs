using DotNetEnv;
using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Repositories.Caching;
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
            var users = _unitOfWork.User.GetAll().Where(u => !u.IsAdmin);

            if (!string.IsNullOrEmpty(userParams.Search))
            {
                users = users.Where(u => u.UserName.Trim().ToLower().Contains(userParams.Search.Trim().ToLower()) 
                || u.PhoneNumber.Contains(userParams.Search) 
                || u.Email.Contains(userParams.Search));
            }

            var usersToDTO = users
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserDTO(u))
                    .AsNoTracking();
                    

            if (userParams.Page != 0 && userParams.PageSize != 0)
                usersToDTO = usersToDTO.Paging(userParams.Page, userParams.PageSize);

            var response = new PagingReponse<UserDTO>(userParams.Page, userParams.PageSize, users.Count(), await usersToDTO.ToListAsync());
            return response;
        } 

        public async Task UploadProfileAsync(Guid id, UserRequestDto request)
        {
            var result = await new UserValidation().ValidateAsync(request);
  
            if (!result.IsValid)           
               throw new ValidationDictionaryException(result.ToDictionary());
            
            var user = await _unitOfWork.User.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var phoneNumbers = _unitOfWork.User
                .GetAll()
                .Where(u => u.PhoneNumber == request.PhoneNumber && u.Id != id);

            if (phoneNumbers.Any())
                throw new ArgumentException("Số điện thoại đã đăng kí");

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
