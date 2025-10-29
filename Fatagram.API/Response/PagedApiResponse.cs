using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Fatagram.API.Response
{
    public class PaginatedApiResponse<TData> : ApiResponse<IEnumerable<TData>>
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
        public bool HasNextPage;

        [JsonPropertyName("hasPreviousPage")]
        public bool HasPreviousPage;

        private PaginatedApiResponse(
            IEnumerable<TData> data,
            int page,
            int pageSize,
            int total,
            string? message
        )
            : base(data: data, message: message)
        {
            Items = data;
            Total = total;
            Page = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)total / pageSize);
        }

        public static PaginatedApiResponse<TData> Create(
            IEnumerable<TData> data,
            int page,
            int pageSize,
            int total,
            string? message
        )
        {
            return new PaginatedApiResponse<TData>(data, page, pageSize, total, message);
        }
    }
}
