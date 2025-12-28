using FoodOrdering.Application.Repositories.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace FoodOrdering.Infrastructure.Repositories.Caching
{
    public class CachingRepo : ICachingService
    {
        private readonly IDistributedCache _cache;

        public CachingRepo(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetAsync<T>(string cacheKey)
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);

            if (string.IsNullOrEmpty(cachedData))
                return default;

            return JsonSerializer.Deserialize<T>(cachedData);
        }

        public async Task RemoveAsync(string cacheKey)
        {
           await _cache.RemoveAsync(cacheKey);
        }

        public async Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiry = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(10)
            };

            var jsonData = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(cacheKey,jsonData, options);
        }
    }
}
