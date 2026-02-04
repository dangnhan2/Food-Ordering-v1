using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Services
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachingRepo _caching;
        private readonly ICloudinaryService _cloudinaryService;
        public const string folder = "Banner";

        public AdvertisementService(
            IUnitOfWork unitOfWork, 
            ICachingRepo cachingService, 
            ICloudinaryService cloudinaryService)
        {
            _unitOfWork = unitOfWork;
            _caching = cachingService;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<IEnumerable<AdvertisementDto>> GetAdvertisementsAsync()
        {
            var cached = await _caching.GetAsync<IEnumerable<AdvertisementDto>>(CacheKeys.ADVERTISEMENT_ACTIVE);

            if (cached != null)
                return cached;

            var advertisements = _unitOfWork.Advertisement.GetAll().Where(a => a.IsActive);

            var advertisementsToDto = await advertisements
                .OrderBy(a => a.StartAt)
                .Select(a => new AdvertisementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    BannerUrl = a.BannerUrl,
                    AdTargetType = a.AdTargetType.ToString(),
                    TargetKey = a.TargetKey,
                    StartAt = a.StartAt.FormatDateTimeOffset(),
                    EndAt = a.EndAt.HasValue ? a.EndAt.Value.FormatDateTimeOffset() : null,
                    IsActive = a.IsActive
                })             
                .AsNoTracking()
                .ToListAsync();

            await _caching.SetAsync(CacheKeys.ADVERTISEMENT_ACTIVE, advertisementsToDto, TimeSpan.FromMinutes(30));

            return advertisementsToDto;
        }

        public async Task<IEnumerable<AdvertisementDto>> GetAdvertisementsByAdminAsync()
        {
            var cached = await _caching.GetAsync<IEnumerable<AdvertisementDto>>(CacheKeys.ADVERTISEMENT_PREFIX);

            if (cached != null)
                return cached;

            var advertisements = _unitOfWork.Advertisement.GetAll();

            var advertisementsToDto = await advertisements
                .OrderBy(a => a.StartAt)
                .Select(a => new AdvertisementDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    BannerUrl = a.BannerUrl,
                    AdTargetType = a.AdTargetType.ToString(),
                    TargetKey = a.TargetKey,
                    StartAt = a.StartAt.FormatDateTimeOffset(),
                    EndAt = a.EndAt.HasValue ? a.EndAt.Value.FormatDateTimeOffset() : null,
                    IsActive = a.IsActive
                })
                .AsNoTracking()
                .ToListAsync();

            await _caching.SetAsync(CacheKeys.ADVERTISEMENT_PREFIX, advertisementsToDto, TimeSpan.FromMinutes(30));

            return advertisementsToDto;
        }

        public async Task AddAdvertisementAsync(AdvertisementRequestDto advertisementRequest)
        {
            var result = await new AdvertisementValidator().ValidateAsync(advertisementRequest);
            if (!result.IsValid) throw new ValidationDictionaryException(result.ToDictionary());

            var isTitleExist = await _unitOfWork.Advertisement.GetAll().AnyAsync(a => a.Title.Replace(" ", "") == advertisementRequest.Title.Replace(" ", ""));

            if (isTitleExist) throw new ArgumentException("Tiêu đề quảng cáo đã tồn tại");

            var newAdvertisement = await MappingAdvertisement(advertisementRequest);

            await _unitOfWork.Advertisement.AddAsync(newAdvertisement);
            await _unitOfWork.SaveChangeAsync();
            await _caching.RemoveAsync(CacheKeys.ADVERTISEMENT_PREFIX);

            if (advertisementRequest.IsActive)
                await _caching.RemoveAsync(CacheKeys.ADVERTISEMENT_ACTIVE);
        }

        public async Task UpdateAdvertisementAsync(Guid adId, AdvertisementRequestDto advertisementRequest)
        {
            var result = await new AdvertisementValidator().ValidateAsync(advertisementRequest);
            if (!result.IsValid) throw new ValidationDictionaryException(result.ToDictionary());

            var advertisement = await _unitOfWork.Advertisement.GetByIdAsync(adId) ?? throw new KeyNotFoundException("Không tìm thấy quảng cáo");

            var isTitleExist = await _unitOfWork.Advertisement.GetAll().AnyAsync(a => a.Title.Replace(" ", "") == advertisementRequest.Title.Replace(" ", "") && a.Id != adId);

            if (isTitleExist) throw new ArgumentException("Tiêu đề quảng cáo đã tồn tại");

            if (advertisementRequest.BannerUrl != null && advertisement.BannerUrl != null)
            {
                await _cloudinaryService.DeleteImage(advertisement.BannerUrl);

                var url = await _cloudinaryService.UploadImage(advertisementRequest.BannerUrl, folder);

                advertisement.BannerUrl = url;
            }
            else if (advertisementRequest.BannerUrl != null)
            {
                var url = await _cloudinaryService.UploadImage(advertisementRequest.BannerUrl, folder);

                advertisement.BannerUrl = url;
            }

            var isActive = advertisement.IsActive;
             
            advertisement.Title = advertisementRequest.Title;
            advertisement.StartAt = advertisementRequest.StartAt;
            advertisement.EndAt = advertisementRequest.EndAt;
            advertisement.AdTargetType = advertisementRequest.AdTargetType;
            advertisement.TargetKey = advertisementRequest.TargetKey;
            advertisement.IsActive = advertisementRequest.IsActive;

            _unitOfWork.Advertisement.Update(advertisement);
            await _unitOfWork.SaveChangeAsync();
            await _caching.RemoveAsync(CacheKeys.ADVERTISEMENT_PREFIX);

            if (isActive != advertisementRequest.IsActive)
                await _caching.RemoveAsync(CacheKeys.ADVERTISEMENT_ACTIVE);
        }

        public async Task RemoveAdvertisementAsync(Guid adId)
        {
            var advertisement = await _unitOfWork.Advertisement.GetByIdAsync(adId) ?? throw new KeyNotFoundException("Không tìm thấy quảng cáo");

            if (advertisement.IsActive)
                throw new InvalidDataException("Quảng cáo đang hoạt động, hãy cập nhật trạng thái quảng cáo trước khi xóa");

            await _cloudinaryService.DeleteImage(advertisement.BannerUrl);

            _unitOfWork.Advertisement.Remove(advertisement);
            await _unitOfWork.SaveChangeAsync();
        }

        #region helper method
        private async Task<Advertisement> MappingAdvertisement(AdvertisementRequestDto advertisementRequest)
        {   
            var url = await _cloudinaryService.UploadImage(advertisementRequest.BannerUrl, folder);

            var advertisement = new Advertisement
            {
                Id = Guid.NewGuid(),
                Title = advertisementRequest.Title,
                BannerUrl = url,
                AdTargetType = advertisementRequest.AdTargetType,
                TargetKey = advertisementRequest.TargetKey,
                StartAt = advertisementRequest.StartAt,
                EndAt = advertisementRequest.EndAt,
                IsActive = advertisementRequest.IsActive,
            };

            return advertisement;
        }
        #endregion
    }
}
