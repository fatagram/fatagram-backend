using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(
            string errorCode,
            string message,
            List<string>? errorMessages = null
        )
            : base(errorCode, message, errorMessages) { }
    }
}
