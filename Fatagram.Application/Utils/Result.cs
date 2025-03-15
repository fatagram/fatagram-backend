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
        public bool IsSuccess { get; set; }

        /// <summary>
        /// The data object
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// The error code
        /// </summary>
        public string? ErrorCode { get; set; } = "UNKNOWN_ERROR";

        private Result(bool isSuccess, T? data, string? errorCode)
        {
            IsSuccess = isSuccess;
            Data = data;

            if (errorCode != null)
                ErrorCode = errorCode;
        }

        /// <summary>
        /// Create a new success result
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success(T data) => new(true, data, null);

        /// <summary>
        /// Create a new failure result
        /// </summary>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        public static Result<T> Failure(string errorCode) => new(false, default, errorCode);

        /// <summary>
        /// Create a new failure result
        /// </summary>
        /// <returns></returns>
        public static Result<T> Failure() => new(false, default, null);
    }
}
