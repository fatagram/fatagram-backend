using System.Text.Json.Serialization;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
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

        public ResponseStatusCode Code { get; set; }

        /// <summary>
        /// The data object
        /// </summary>
        public T? Data { get; }

        public string? Message { get; }

        /// <summary>
        /// The error code
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// The error message
        /// </summary>
        public string? ErrorMessage { get; }

        private Result(
            bool isSuccess = true,
            T? data = default,
            string? message = null,
            string? errorCode = null,
            string? errorMessage = null,
            ResponseStatusCode code = ResponseStatusCode.Success
        )
        {
            IsSuccess = isSuccess;
            Data = data;
            Message = message;
            Code = code;

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
        public static Result<T> Success(
            ResponseStatusCode statusCode = ResponseStatusCode.Success,
            T? data = default,
            string? message = null
        ) => new(true, data, message, null, null, statusCode);

        /// <summary>
        /// Create a new failure result
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static Result<T> Failure(
            ResponseStatusCode statusCode = ResponseStatusCode.InternalServerError,
            string? errorCode = null,
            string? errorMessage = null
        ) => new(false, default, null, errorCode, errorMessage, statusCode);
    }
}
