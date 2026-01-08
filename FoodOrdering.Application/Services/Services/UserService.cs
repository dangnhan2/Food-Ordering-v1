using DotNetEnv;
using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<User> _userManager;
 
        public UserService(
            IUnitOfWork unitOfWork, 
            ICloudinaryService cloudinaryService, 
            ICachingService cachingService, 
            UserManager<User> userManager,
            ITokenService tokenService)
        {
            Env.Load();
            _unitOfWork = unitOfWork;
            _cloudinaryService = cloudinaryService;
            _defaultAvatar = Env.GetString("DEFAULT_AVATAR");
            _cachingService = cachingService;
            _userManager = userManager;
        }

        public async Task<PagingReponse<UserDTO>> GetAllAsync(UserParams userParams)
        {
            var users = _unitOfWork.User.GetAll().Where(u => !u.IsAdmin);

            if (!string.IsNullOrEmpty(userParams.Search))
            {
                users = users.Where(u => 
                EF.Functions.Like(u.UserName, $"%{userParams.Search}")
                || u.PhoneNumber ==  userParams.Search
                || u.Email ==  userParams.Search);
            }

            var usersToDTO = users
                    .OrderByDescending(u => u.CreatedAt)
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        PhoneNumber = u.PhoneNumber,
                        ImageUrl = u.ImageUrl,
                        Email = u.Email,
                        IsActive = u.LockoutEnd.HasValue ? false : true,
                        TotalAmountInMonth = u.Orders
                        .Where(o => o.User.Id == u.Id && o.OrderDate.Month == DateTimeOffset.UtcNow.Month)
                        .Sum(o => o.TotalAmount),
                        TotalAmountInYear = u.Orders
                        .Where(o => o.User.Id == u.Id && o.OrderDate.Year == DateTimeOffset.UtcNow.Year)
                        .Sum(o => o.TotalAmount)

                    })
                    .AsNoTracking();
                    

            if (userParams.Page != 0 && userParams.PageSize != 0)
                usersToDTO = usersToDTO.Paging(userParams.Page, userParams.PageSize);

            var response = new PagingReponse<UserDTO>(userParams.Page, userParams.PageSize, users.Count(), await usersToDTO.ToListAsync());
            return response;
        } 

        public async Task UploadProfileAsync(Guid userId, UserRequestDto request)
        {
            var result = await new UserValidation().ValidateAsync(request);
  
            if (!result.IsValid)           
               throw new ValidationDictionaryException(result.ToDictionary());
            
            var user = await _unitOfWork.User.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var phoneNumbers = _unitOfWork.User
                .GetAll()
                .Where(u => u.PhoneNumber == request.PhoneNumber && u.Id != userId);

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

        public async Task<UserDTO> GetUserByIdAsync(Guid userId)
        {
            var cacheKey = CacheKeys.UserDetail(userId);
            var cached = await _cachingService.GetAsync<UserDTO>(cacheKey);
            if (cached != null)
                return cached;

            var user = await _unitOfWork.User.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            var response = new UserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                ImageUrl = user.ImageUrl,
                Email = user.Email,
                IsActive = user.LockoutEnd.HasValue ? false : true,
                TotalAmountInMonth = user.Orders
                        .Where(o => o.User.Id == user.Id && o.OrderDate.Month == DateTimeOffset.UtcNow.Month)
                        .Sum(o => o.TotalAmount),
                TotalAmountInYear = user.Orders
                        .Where(o => o.User.Id == user.Id && o.OrderDate.Year == DateTimeOffset.UtcNow.Year)
                        .Sum(o => o.TotalAmount)
            };

            await _cachingService.SetAsync(cacheKey, response, TimeSpan.FromHours(12));
            return response;
        }

        public async Task BanUserAsync(Guid userId)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            await _userManager.SetLockoutEnabledAsync(user, true);
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(10));

            var refreshTokens = _unitOfWork.RefreshToken
                .GetAll()
                .Where(rt => rt.UserId == userId);

            foreach(var rt in refreshTokens)
            {
                rt.IsRevoked = true;
            }

            await _unitOfWork.SaveChangeAsync();
        }

        public async Task UnBanUserAsync(Guid userId)
        {
            var user = await _unitOfWork.User.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("Người dùng không tồn tại");

            await _userManager.SetLockoutEndDateAsync(user, null);
        }
    }
}
