using System.Net;
using System.Text.Json.Serialization;
using Fatagram.API.Response;

namespace Fatagram.API.Utils
{
    /// <summary>
    /// Response from API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CursorPaginatedApiResponse<TCursor, TData>
    {
        [JsonPropertyName("items")]
        public IEnumerable<TData> Items { get; set; } = new List<TData>();

        [JsonPropertyName("nextCursor")]
        public TCursor? NextCursor { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        private CursorPaginatedApiResponse(IEnumerable<TData> items, TCursor? nextCursor, int total)
        {
            Items = items;
            NextCursor = nextCursor;
            Total = total;
        }

        public static CursorPaginatedApiResponse<TCursor, TData> Create(
            IEnumerable<TData> items,
            TCursor? nextCursor,
            int total
        )
        {
            return new CursorPaginatedApiResponse<TCursor, TData>(items, nextCursor, total);
        }
    }
}
