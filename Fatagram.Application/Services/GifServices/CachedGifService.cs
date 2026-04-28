using System;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Gif;
using Fatagram.Application.Services.GifServices.Interfaces;
using Fatagram.Infrastructure.Cache;

namespace Fatagram.Application.Services.GifServices
{
    public class CachedGifService(IGifService inner, ICacheService cacheService) : IGifService
    {
        private readonly IGifService _inner = inner;
        private readonly ICacheService _cacheService = cacheService;

        public async Task<GifResponseDto> SearchGifsAsync(
            string query,
            int limit = 20,
            string? pos = null
        )
        {
            var normalizedQuery = query.Trim().ToLowerInvariant();
            var key = $"v2:gifs:search:{normalizedQuery}:{limit}:{pos ?? "0"}";

            if (await _cacheService.ExistsAsync(key))
            {
                var cachedResult = await _cacheService.GetAsync<GifResponseDto>(key);
                if (cachedResult != null)
                {
                    return cachedResult;
                }
            }

            var result = await _inner.SearchGifsAsync(query, limit, pos);

            if (result != null && result.Gifs.Count > 0)
            {
                await _cacheService.SetAsync(key, result, TimeSpan.FromDays(7));
            }
            else
            {
                var fallbackKey = "v2:gifs:fallback:trending";
                if (await _cacheService.ExistsAsync(fallbackKey))
                {
                    var fallbackResult = await _cacheService.GetAsync<GifResponseDto>(fallbackKey);
                    if (fallbackResult != null)
                    {
                        return fallbackResult;
                    }
                }
            }

            return result ?? new GifResponseDto();
        }

        public async Task<GifResponseDto> GetTrendingGifsAsync(int limit = 20, string? pos = null)
        {
            var key = $"v2:gifs:trending:{limit}:{pos ?? "0"}";

            if (await _cacheService.ExistsAsync(key))
            {
                var cachedResult = await _cacheService.GetAsync<GifResponseDto>(key);
                if (cachedResult != null)
                {
                    return cachedResult;
                }
            }

            var result = await _inner.GetTrendingGifsAsync(limit, pos);

            var fallbackKey = "v2:gifs:fallback:trending";
            if (result != null && result.Gifs.Count > 0)
            {
                await _cacheService.SetAsync(key, result, TimeSpan.FromHours(12));

                // Save to fallback permanently (or 30 days) if it's the first page
                if (string.IsNullOrEmpty(pos) || pos == "0")
                {
                    await _cacheService.SetAsync(fallbackKey, result, TimeSpan.FromDays(30));
                }
            }
            else
            {
                // Fallback mechanism: if API fails, try to return stored fallback
                if (await _cacheService.ExistsAsync(fallbackKey))
                {
                    var fallbackResult = await _cacheService.GetAsync<GifResponseDto>(fallbackKey);
                    if (fallbackResult != null)
                    {
                        return fallbackResult;
                    }
                }
            }

            return result ?? new GifResponseDto();
        }
    }
}
