using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Gif;
using Fatagram.Application.Services.GifServices.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.GifServices
{
    public class GiphyGifService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<GiphyGifService> logger
    ) : IGifService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _apiKey =
            configuration["Giphy:ApiKey"] ?? "xQ8d1Qy8r902j9K0E4gB1S1873M288";
        private readonly ILogger<GiphyGifService> _logger = logger;

        public async Task<GifResponseDto> SearchGifsAsync(
            string query,
            int limit = 20,
            string? pos = null
        )
        {
            int offset = 0;
            if (!string.IsNullOrEmpty(pos) && int.TryParse(pos, out int parsedOffset))
            {
                offset = parsedOffset;
            }

            var url =
                $"https://api.giphy.com/v1/gifs/search?api_key={_apiKey}&q={Uri.EscapeDataString(query)}&limit={limit}&offset={offset}";
            return await FetchGifsAsync(url);
        }

        public async Task<GifResponseDto> GetTrendingGifsAsync(int limit = 20, string? pos = null)
        {
            int offset = 0;
            if (!string.IsNullOrEmpty(pos) && int.TryParse(pos, out int parsedOffset))
            {
                offset = parsedOffset;
            }

            var url =
                $"https://api.giphy.com/v1/gifs/trending?api_key={_apiKey}&limit={limit}&offset={offset}";
            return await FetchGifsAsync(url);
        }

        private async Task<GifResponseDto> FetchGifsAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var giphyResponse = JsonSerializer.Deserialize<GiphyApiResponse>(content, options);

                var result = new GifResponseDto();
                if (giphyResponse != null)
                {
                    if (giphyResponse.Pagination != null)
                    {
                        int nextOffset =
                            giphyResponse.Pagination.Offset + giphyResponse.Pagination.Count;
                        if (nextOffset < giphyResponse.Pagination.Total_Count)
                        {
                            result.Next = nextOffset.ToString();
                        }
                    }

                    if (giphyResponse.Data != null)
                    {
                        foreach (var item in giphyResponse.Data)
                        {
                            var gifUrl = item.Images?.Original?.Url ?? "";
                            var previewUrl = item.Images?.Fixed_Height_Small?.Url ?? gifUrl;

                            result.Gifs.Add(
                                new GifDto
                                {
                                    Id = item.Id ?? "",
                                    Title = item.Title ?? "",
                                    Url = gifUrl,
                                    PreviewUrl = previewUrl,
                                }
                            );
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GIFs from Giphy API");
                return new GifResponseDto();
            }
        }

        private class GiphyApiResponse
        {
            public List<GiphyResult>? Data { get; set; }
            public GiphyPagination? Pagination { get; set; }
        }

        private class GiphyResult
        {
            public string? Id { get; set; }
            public string? Title { get; set; }
            public GiphyImages? Images { get; set; }
        }

        private class GiphyImages
        {
            public GiphyMedia? Original { get; set; }
            public GiphyMedia? Fixed_Height_Small { get; set; }
        }

        private class GiphyMedia
        {
            public string? Url { get; set; }
        }

        private class GiphyPagination
        {
            public int Total_Count { get; set; }
            public int Count { get; set; }
            public int Offset { get; set; }
        }
    }
}
