using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.API.Utils.Response;
using Fatagram.Shared.Common;

namespace Fatagram.API.Utils.Response
{
    public class ErrorDetails
    {
        [JsonPropertyName("code")]
        public string Code { get; set; } = "UNKNOWN_ERROR";

        [JsonPropertyName("detail")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Detail { get; set; }
    }

    public class ErrorResponse : BaseResponse
    {
        [JsonPropertyName("error")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorDetails? Error { get; set; }

        [JsonPropertyName("errors")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorDetails[]? Errors { get; set; }

        private ErrorResponse(ErrorDetails? error, ErrorDetails[]? errors, string? message)
            : base(false, message, null)
        {
            Error = error;
            Errors = errors;
        }

        public static ErrorResponse Create(
            ErrorDetails? error,
            ErrorDetails[]? errors = null,
            string? message = null
        )
        {
            return new ErrorResponse(error, errors, message);
        }

        public static ErrorResponse Create(ErrorDetails[]? errors = null)
        {
            return new ErrorResponse(null, errors, null);
        }

        public static ErrorResponse Create(ErrorDetails? error)
        {
            return new ErrorResponse(error, null, null);
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
