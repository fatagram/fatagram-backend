using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task<T?> Get<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? absoluteExpirationRelativeToNow = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);
    }
}
