using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Cache
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);

        Task HashSetAsync(string key, string field, string value);
        Task<string?> HashGetAsync(string key, string field);
        Task<Dictionary<string, string>> HashGetAllAsync(string key);

        Task SetAddAsync(string key, string value);
        Task<IEnumerable<string>> SetMembersAsync(string key);

        Task<IEnumerable<string>> GetKeysAsync(string pattern);
    }
}
