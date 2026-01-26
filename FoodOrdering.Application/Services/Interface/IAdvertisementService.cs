using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IAdvertisementService
    {
        public Task<IEnumerable<AdvertisementDto>> GetAdvertisementsByAdminAsync();
        public Task<IEnumerable<AdvertisementDto>> GetAdvertisementsAsync();
        public Task AddAdvertisementAsync(AdvertisementRequestDto advertisementRequest);
        public Task UpdateAdvertisementAsync(Guid adId, AdvertisementRequestDto advertisementRequest);
        public Task RemoveAdvertisementAsync(Guid adId);
    }
}
