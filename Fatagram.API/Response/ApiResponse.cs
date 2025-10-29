using System.Net;
using System.Text.Json.Serialization;
using Fatagram.API.Response;

namespace Fatagram.API.Utils
{
    /// <summary>
    /// Response from API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; set; }

        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }

        // [JsonPropertyName("error")]
        // [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        // public ApiError? Error { get; set; }

        private ApiResponse(T? data, string? message, ApiError? error)
        {
            Data = data;
            Message = message;
        }

        public static ApiResponse<T> Create(T? data = default, string? message = null)
        {
            return new ApiResponse<T>(data, message, null);
        }

        public static ApiResponse<T> Failure(ApiError? error = null)
        {
            return new ApiResponse<T>(default, null, error);
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
