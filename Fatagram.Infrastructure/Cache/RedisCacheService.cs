using System.Text.Json;
using Fatagram.Infrastructure.Cache;
using StackExchange.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
        _db = redis.GetDatabase();
    }

    // --- Basic String/JSON ---
    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _db.StringGetAsync(key);
        if (data.IsNullOrEmpty)
            return default;
        return JsonSerializer.Deserialize<T>(data!);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var jsonData = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, jsonData, expiration);
    }

    // --- Hash Operations (Sức mạnh cho vụ Seen) ---
    public async Task HashSetAsync(string key, string field, string value)
    {
        await _db.HashSetAsync(key, field, value);
    }

    public async Task<string?> HashGetAsync(string key, string field)
    {
        var result = await _db.HashGetAsync(key, field);
        return result.HasValue ? result.ToString() : null;
    }

    public async Task<Dictionary<string, string>> HashGetAllAsync(string key)
    {
        var entries = await _db.HashGetAllAsync(key);
        return entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
    }

    // --- Set Operations (Sức mạnh cho Member List) ---
    public async Task SetAddAsync(string key, string value)
    {
        await _db.SetAddAsync(key, value);
    }

    public async Task<IEnumerable<string>> SetMembersAsync(string key)
    {
        var members = await _db.SetMembersAsync(key);
        return members.Select(x => x.ToString());
    }

    public async Task RemoveAsync(string key) => await _db.KeyDeleteAsync(key);

    public async Task<bool> ExistsAsync(string key) => await _db.KeyExistsAsync(key);

    public async Task<IEnumerable<string>> GetKeysAsync(string pattern)
    {
        var keys = new List<string>();

        foreach (var endpoint in _redis.GetEndPoints())
        {
            var server = _redis.GetServer(endpoint);
            await foreach (var key in server.KeysAsync(pattern: pattern))
            {
                keys.Add(key.ToString());
            }
        }
        return keys;
    }

    public async Task<long> IncrementAsync(string key, long value = 1)
    {
        return await _db.StringIncrementAsync(key, value);
    }

    public async Task<long> HashIncrementAsync(string key, string field, long value = 1)
    {
        return await _db.HashIncrementAsync(key, field, value);
    }

    public async Task<bool> TryAcquireLockAsync(string key, TimeSpan expiration)
    {
        return await _db.StringSetAsync(key, "1", expiration, when: When.NotExists);
    }
}
