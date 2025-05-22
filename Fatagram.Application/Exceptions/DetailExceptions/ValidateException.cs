using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class ValidateException : BadRequestException
    {
        public ValidateException(string errorCode, List<string>? errorCodes, string message)
            : base(errorCode, errorCodes, message)
        { }
    }
}