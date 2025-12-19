using System.Net;
using System.Text.Json.Serialization;

namespace Fatagram.API.Utils.Response
{
    /// <summary>
    /// Response from API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CursorResponse<TCursor, TData> : ApiResponse<IEnumerable<TData>>
        where TCursor : struct
    {
        [JsonPropertyName("nextCursor")]
        public TCursor? NextCursor { get; set; }

        [JsonPropertyName("hasNext")]
        public bool HasNext { get; set; }

        private CursorResponse(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            bool hasNext,
            string? message
        )
            : base(data: data, message: message)
        {
            NextCursor = nextCursor;
            HasNext = hasNext;
        }

        public static CursorResponse<TCursor, TData> Create(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            bool hasNext,
            string? message
        )
        {
            return new CursorResponse<TCursor, TData>(data, nextCursor, hasNext, message);
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
