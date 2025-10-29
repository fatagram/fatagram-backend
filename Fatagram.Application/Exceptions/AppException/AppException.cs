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
        public List<string>? ErrorMessages { get; set; }

        // public List<string>? ErrorMessages { get; set; }

        public AppException(string errorCode, string? message, List<string>? errorMessages = null)
            : base(message)
        {
            ErrorCode = errorCode;
            ErrorMessages = errorMessages;
        }
    }
}
