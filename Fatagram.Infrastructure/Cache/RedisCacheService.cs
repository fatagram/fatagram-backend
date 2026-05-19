using System.Text.Json;
using StackExchange.Redis;

namespace Fatagram.Infrastructure.Cache;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IConnectionMultiplexer _redis = redis;
    private readonly IDatabase _db = redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key)
    {
        var data = await _db.StringGetAsync(key);
        if (data.IsNullOrEmpty)
            return default;
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

    public async Task<bool> SortedSetAddAsync<T>(string key, T value, double score)
    {
        return await _db.SortedSetAddAsync(key, ToRedisValue(value), score);
    }

    public async Task<List<T>> SortedSetRangeByScoreAsync<T>(
        string key,
        double start = double.NegativeInfinity,
        double stop = double.PositiveInfinity,
        Exclude exclude = Exclude.None,
        Order order = Order.Ascending,
        long skip = 0,
        long take = -1
    )
    {
        var members = await _db.SortedSetRangeByScoreAsync(
            key,
            start,
            stop,
            exclude,
            order,
            skip,
            take
        );

        return members.Select(FromRedisValue<T>).ToList();
    }

    public async Task<bool> SortedSetRemoveAsync<T>(string key, T value)
    {
        return await _db.SortedSetRemoveAsync(key, ToRedisValue(value));
    }

    public async Task<long?> SortedSetRankAsync<T>(string key, T value, bool desc = false)
    {
        var order = desc ? Order.Descending : Order.Ascending;
        return await _db.SortedSetRankAsync(key, ToRedisValue(value), order);
    }

    public async Task<List<(T Value, double Score)>> SortedSetRangeByScoreWithCursorAsync<T>(
        string key,
        double? cursor = null,
        int? limit = null,
        bool desc = true
    )
    {
        double start = desc
            ? (cursor ?? double.PositiveInfinity)
            : (cursor ?? double.NegativeInfinity);

        double stop = desc ? double.NegativeInfinity : double.PositiveInfinity;

        Exclude exclude = cursor.HasValue ? Exclude.Start : Exclude.None;

        var members = await _db.SortedSetRangeByScoreWithScoresAsync(
            key,
            start: start,
            stop: stop,
            exclude: exclude,
            order: desc ? Order.Descending : Order.Ascending,
            take: limit ?? -1
        );

        return members.Select(m => (FromRedisValue<T>(m.Element), m.Score)).ToList();
    }

    public async Task<long> PublishAsync<T>(string channel, T message)
    {
        var subscriber = _redis.GetSubscriber();
        var json = JsonSerializer.Serialize(message);
        return await subscriber.PublishAsync(RedisChannel.Literal(channel), json);
    }

    public async Task SubscribeAsync<T>(string channel, Action<T> handler)
    {
        var subscriber = _redis.GetSubscriber();
        await subscriber.SubscribeAsync(
            RedisChannel.Literal(channel),
            (redisChannel, value) =>
            {
                var message = JsonSerializer.Deserialize<T>(value!);
                if (message != null)
                    handler(message);
            }
        );
    }

    public async Task ExecuteBatchAsync(Action<ICacheService> batchAction)
    {
        var tran = _db.CreateTransaction();
    }

    private RedisValue ToRedisValue<T>(T value)
    {
        if (value == null)
            return RedisValue.Null;

        if (value is string s)
            return s;
        if (value is Guid g)
            return g.ToString();
        if (value is int || value is long || value is double)
            return value.ToString();

        return JsonSerializer.Serialize(value);
    }

    private T FromRedisValue<T>(RedisValue value)
    {
        if (value.IsNull)
            return default!;

        if (typeof(T) == typeof(string))
            return (T)(object)value.ToString();
        if (typeof(T) == typeof(Guid))
            return (T)(object)Guid.Parse(value.ToString());

        return JsonSerializer.Deserialize<T>(value!)!;
    }

    public async Task HashSetAsync(string key, HashEntry[] hashEntries)
    {
        await _db.HashSetAsync(key, hashEntries);
    }

    public async Task<bool> SortedSetAddAsync(string key, SortedSetEntry[] entries)
    {
        var count = await _db.SortedSetAddAsync(key, entries);
        return count > 0;
    }

    public async Task SetIfNotExistsAsync(string key, string value, TimeSpan expiration)
    {
        await _db.StringSetAsync(key, value, expiration, when: When.NotExists);
    }
}
