using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class AppException : Exception
    {
        public string ErrorCode { get; set; }
        public List<string>? ErrorCodes { get; set; }
        public AppException(string errorCode, List<string>? errorCodes, string? message)
            : base(message)
        {
            ErrorCode = errorCode;
            ErrorCodes = errorCodes;
        }
    }
}
