using System.Net;
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
        public int Status { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        [JsonPropertyName("title")]
        public string Title { get; set; } = "Success";

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
        public ApiResponse(int status, string title, T? data = default, ApiError? error = null, string? message = null)
        {
            Status = status;
            Title = title;
            Data = data;
            Error = error;
            Message = message;
        }

        /// <summary>
        /// Suucess response
        /// </summary>
        /// <param name="data"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> Success(
            string? title = null,
            T? data = default,
            string? message = null) 
           => new ApiResponse<T>((int)HttpStatusCode.OK, title ?? "Success", data, null, message);
        
        /// <summary>
        /// BadRequest response
        /// </summary>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> BadRequest(
            string? title = null,
            T? data = default,
            ApiError? error = null,
            string? message = null)
            => new ApiResponse<T>((int)HttpStatusCode.BadRequest, title ?? "Bad Request", data, error, message);
        
        /// <summary>
        /// NotFound response
        /// </summary>
        /// <param name="title"></param>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> NotFound(
            string? title = null, 
            T? data = default, 
            ApiError? error = null, 
            string? message = null)
            => new ApiResponse<T>((int)HttpStatusCode.NotFound, title ?? "Not Found", data, error, message);

        /// <summary>
        /// Unauthorized response
        /// </summary>
        /// <param name="title"></param>
        /// <param name="data"></param>
        /// <param name="error"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ApiResponse<T> InternalServerError(
            string? title = null, 
            T? data = default,  
            ApiError? error = null,
            string? message = null)
            => new ApiResponse<T>((int)HttpStatusCode.InternalServerError, title ?? "Internal Server Error", data, error, message);
    }

    // Class ApiError
    public class ApiError
    {
        /// <summary>
        /// Code
        /// </summary>
        [JsonPropertyName("code")]
        public IEnumerable<string?>? Code { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}