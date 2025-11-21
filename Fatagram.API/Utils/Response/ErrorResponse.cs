using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.API.Utils;

namespace Fatagram.API.Response
{
    public class ErrorResponse : BaseResponse
    {
        [JsonPropertyName("code")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Code { get; set; }

        public object? Errors { get; set; }

        private ErrorResponse(string code, object? errors, string? message)
            : base(false, message, null)
        {
            Code = code;
            Errors = errors;
        }

        public static ErrorResponse Create(
            string code,
            object? errors = null,
            string? message = null
        )
        {
            return new ErrorResponse(code, errors, message);
        }
    }
}
