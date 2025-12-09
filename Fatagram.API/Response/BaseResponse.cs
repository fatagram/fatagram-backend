using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fatagram.API.Response
{
    public class BaseResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; } = string.Empty;

        [JsonPropertyName("traceId")]
        public string? TraceId { get; set; }

        protected BaseResponse(bool success = true, string? message = null, string? traceId = null)
        {
            Success = success;
            Message = message;
            TraceId = traceId;
        }

        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }
    }
}
