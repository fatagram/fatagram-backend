using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class AppException : Exception
    {
        public string? Code { get; }
        public override string Message { get; }

        public AppException(string? code = null, string? message = null) : base(message)
        {
            Code = code;
            Message = message ?? "";
        }
    }
}
