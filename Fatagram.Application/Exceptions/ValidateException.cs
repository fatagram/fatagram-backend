using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class ValidateException : AppException
    {
        public List<string>? Errors { get; }
        public ValidateException(string? code = null, string? message = null, List<string>? errors = null)
            : base(code, message)
        {
            Errors = errors;
        }
    }
}
