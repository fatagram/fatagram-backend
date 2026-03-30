using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Fatagram.Infrastructure.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<T?> Get<T>(string key)
        {
            var jsonData = await _distributedCache.GetStringAsync(key);
            if (string.IsNullOrEmpty(jsonData))
                return default;
            return JsonSerializer.Deserialize<T>(jsonData, _jsonOptions);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(1),
                SlidingExpiration = TimeSpan.FromMinutes(10),
            };

            var jsonData = JsonSerializer.Serialize(value, _jsonOptions);
            await _distributedCache.SetStringAsync(key, jsonData, options);
        }

        public async Task RemoveAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            var data = await _distributedCache.GetAsync(key);
            return data != null;
        }
    }
}
