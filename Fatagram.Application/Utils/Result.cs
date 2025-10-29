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
        public enum StatusCode
        {
            Success = 200,
            BadRequest = 400,
            Unauthorized = 401,
            Forbidden = 403,
            NotFound = 404,
            InternalServerError = 500,
        }

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
        public static Result<T> Success(T? data = default, string? message = null) =>
            new(true, data, message);

        /// <summary>
        /// Create a new failure result
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static Result<T> BadRequest(string? errorCode = null, string? errorMessage = null) =>
            new(false, default, null, errorCode, errorMessage, ResponseStatusCode.BadRequest);

        public static Result<T> Unauthorized(
            string? errorCode = null,
            string? errorMessage = null
        ) => new(false, default, null, errorCode, errorMessage, ResponseStatusCode.Unauthorized);

        public static Result<T> Forbidden(string? errorCode = null, string? errorMessage = null) =>
            new(false, default, null, errorCode, errorMessage, ResponseStatusCode.Forbidden);

        public static Result<T> NotFound(string? errorCode = null, string? errorMessage = null) =>
            new(false, default, null, errorCode, errorMessage, ResponseStatusCode.NotFound);

        public static Result<T> InternalServerError(
            string? errorCode = null,
            string? errorMessage = null
        ) =>
            new(
                false,
                default,
                null,
                errorCode,
                errorMessage,
                ResponseStatusCode.InternalServerError
            );
    }
}
