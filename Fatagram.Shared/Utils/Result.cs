using System.Text.Json.Serialization;

namespace Fatagram.Shared.Utils
{
    /// <summary>
    /// A generic result class to return a result with a data object or an error code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        /// <summary>
        /// Whether the operation was successful
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// The data object
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Data { get; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; }

        /// <summary>
        /// The error code
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorCode { get; }

        /// <summary>
        /// The error message
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ErrorMessage { get; }

        private Result(bool isSuccess = true, T? data = default, string? message = null, string? errorCode = null, string? errorMessage = null)
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;

            if (!isSuccess)
            {
                ErrorCode = errorCode;
                ErrorMessage = errorMessage;
            }
        }

        /// <summary>
        /// Create a new success result
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success(T? data = default, string? message = null) => new(true, data, message);

        /// <summary>
        /// Create a new failure result
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static Result<T> Failure(string? errorCode = null, string? errorMessage = null) => new(false, default, null, errorCode, errorMessage);
    }
}
