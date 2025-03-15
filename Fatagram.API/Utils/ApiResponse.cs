using System.Text.Json.Serialization;

namespace Fatagram.API.Utils
{
    /// <summary>
    /// Response from API
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Status code
        /// </summary>
        [JsonPropertyName("status")]
        public string Status { get; set; }

        /// <summary>
        /// Data
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }


        /// <summary>
        /// Message
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }


        /// <summary>
        /// Error
        /// </summary>
        [JsonPropertyName("error")]
        public ApiError? Error { get; set; }


        // Constructor
        public ApiResponse(string status, T? data, string? message)
        {
            Status = status;
            Data = data;
            Message = message;
            Error = null; // Không có lỗi
        }

        public ApiResponse(string status, T? data)
        {
            Status = status;
            Data = data;
            Error = null; // Không có lỗi
        }


        // Constructor
        public ApiResponse(string status, ApiError error, string? message)
        {
            Status = status;
            Error = error;
            Message = message;
            Data = default; // Không có dữ liệu
        }

        // Constructor
        public ApiResponse(string status, ApiError error)
        {
            Status = status;
            Error = error;
            Data = default; // Không có dữ liệus
        }
    }


    // Class ApiError
    public class ApiError
    {
        [JsonPropertyName("code")]
        public string?[]? Code { get; set; }

        [JsonPropertyName("message")]
        public string?[]? Message { get; set; }
    }
}