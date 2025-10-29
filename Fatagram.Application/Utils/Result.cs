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
        public ResponseStatusCode Code { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }

        protected Result(ResponseStatusCode code, T? data = default, string? message = null)
        {
            Code = code;
            Data = data;
            Message = message;
        }

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
