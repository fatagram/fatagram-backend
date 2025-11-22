using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions
{
    public class AppException(Error error, List<Error>? errors = null) : Exception(error.Message)
    {
        public Error Error { get; } = error;
        public List<Error>? Errors { get; set; } = errors;

        public AppException(string errorCode, string? errorMessage, List<Error>? errors = null)
            : this(new Error(errorCode, errorMessage ?? string.Empty), errors) { }
    }
}
