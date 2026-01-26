using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories.Caching
{
    public interface ICachingRepo
    {
        public Task<T?> GetAsync<T>(string cacheKey);
        public Task SetAsync<T>(string cacheKey, T value, TimeSpan? expiry = null);
        public Task RemoveAsync(string cacheKey);
    }
}
