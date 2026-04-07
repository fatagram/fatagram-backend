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
        // If caller expects a raw string, return the Redis value as-is to avoid
        // JSON deserialization errors when the stored value is a JSON number.
        if (typeof(T) == typeof(string))
        {
            object? obj = data.ToString();
            return (T?)obj;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(data!);
        }
        catch (JsonException)
        {
            // Fallback: try to convert simple primitive values stored as plain
            // strings/numbers into the requested type (e.g., int, long, bool).
            try
            {
                var str = data.ToString();
                var converted = (T?)Convert.ChangeType(str, typeof(T));
                return converted;
            }
            catch
            {
                return default;
            }
        }
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

    public async Task<long> SetRemoveAsync(string key, string value)
    {
        await _db.SetRemoveAsync(key, value);
        return await _db.SetLengthAsync(key);
    }

    public Task<long> SetLengthAsync(string key)
    {
        return _db.SetLengthAsync(key);
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

    public async Task<bool> KeyExpireAsync(string key, TimeSpan expiration)
    {
        return await _db.KeyExpireAsync(key, expiration);
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

    public async Task<long> ListRightPushAsync<T>(string key, T value)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        return await _db.ListRightPushAsync(key, serializedValue);
    }

    public async Task<long> ListLeftPushAsync<T>(string key, T value)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        return await _db.ListLeftPushAsync(key, serializedValue);
    }

    public async Task<List<T>> ListRangeAsync<T>(string key, long start, long stop)
    {
        var values = await _db.ListRangeAsync(key, start, stop);
        return values.Select(v => JsonSerializer.Deserialize<T>(v!)).ToList()!;
    }

    public async Task ListTrimAsync(string key, long start, long stop)
    {
        await _db.ListTrimAsync(key, start, stop);
    }

    public async Task<long> ListLengthAsync(string key)
    {
        return await _db.ListLengthAsync(key);
    }

    public async Task HashObjectSetAsync<T>(string key, string field, T value)
    {
        var serializedValue = JsonSerializer.Serialize(value);
        await _db.HashSetAsync(key, field, serializedValue);
    }

    public async Task<T?> HashObjectGetAsync<T>(string key, string field)
    {
        var value = await _db.HashGetAsync(key, field);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value.ToString()) : default(T);
    }
}
