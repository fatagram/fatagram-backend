using System.Net;
using System.Text.Json.Serialization;
using Fatagram.API.Response;

namespace Fatagram.API.Utils
{
    /// <summary>
    /// Response from API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T> : BaseResponse
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        protected ApiResponse(T? data, string? message)
            : base(true, message, null)
        {
            Data = data;
            Message = message;
        }

        public static ApiResponse<T> Create(T? data = default, string? message = null)
        {
            return new ApiResponse<T>(data, message);
        }
    }
}
