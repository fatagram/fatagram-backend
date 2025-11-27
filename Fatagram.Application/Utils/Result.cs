using System.Text.Json.Serialization;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class Result
    {
        public ResponseStatusCode Code { get; set; }
        public string? Message { get; set; }

        protected Result(ResponseStatusCode code, string? message = null)
        {
            Code = code;
            Message = message;
        }

        public static Result Create(ResponseStatusCode? code = null, string? message = null)
        {
            return new Result(code ?? ResponseStatusCode.Success, message);
        }
    }

    /// <summary>
    /// A generic result class to return a result with a data object or an error code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>(ResponseStatusCode code, T? data = default, string? message = null)
        : Result(code, message)
    {
        public T? Data { get; set; } = data;

        public static Result<T> Create(
            ResponseStatusCode? code = null,
            T? data = default,
            string? message = null
        )
        {
            return new Result<T>(code ?? ResponseStatusCode.Success, data, message);
        }
    }
}
