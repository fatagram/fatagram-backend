using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Validation
{
    public class AppValidationException : Exception
    {
        public string ErrorCode { get; }
        public AppValidationException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
