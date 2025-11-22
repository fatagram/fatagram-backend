using System.Net;
using System.Text.Json.Serialization;

namespace Fatagram.API.Utils.Response
{
    /// <summary>
    /// Response from API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CursorResponse<TCursor, TData> : ApiResponse<IEnumerable<TData>>
    {
        [JsonPropertyName("nextCursor")]
        public TCursor? NextCursor { get; set; }

        [JsonPropertyName("hasNextCursor")]
        public bool HasNextCursor => NextCursor != null;

        [JsonPropertyName("total")]
        public int Total { get; set; }

        private CursorResponse(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            int total,
            string? message
        )
            : base(data: data, message: message)
        {
            NextCursor = nextCursor;
            Total = total;
        }

        public static CursorResponse<TCursor, TData> Create(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            int total,
            string? message
        )
        {
            return new CursorResponse<TCursor, TData>(data, nextCursor, total, message);
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
