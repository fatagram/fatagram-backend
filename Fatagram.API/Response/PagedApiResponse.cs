using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fatagram.API.Response
{
    public class PaginatedApiResponse<TData>
    {
        [JsonPropertyName("items")]
        public IEnumerable<TData> Items { get; set; } = new List<TData>();

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("hasNextPage")]
        public bool HasNextPage => Page < TotalPages;

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage => Page > 1;

        private PaginatedApiResponse(IEnumerable<TData> items, int total, int page, int pageSize)
        {
            Items = items;
            Total = total;
            Page = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)total / pageSize);
        }

        public static PaginatedApiResponse<TData> Create(
            IEnumerable<TData> items,
            int total,
            int page,
            int pageSize
        )
        {
            return new PaginatedApiResponse<TData>(items, total, page, pageSize);
        }
    }
}
