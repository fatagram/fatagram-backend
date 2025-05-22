using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string errorCode, List<string>? errorCodes, string message) 
            : base(errorCode, errorCodes, message)
        { }
    }
}