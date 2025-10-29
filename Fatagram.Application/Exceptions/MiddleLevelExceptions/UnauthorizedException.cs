using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.MiddleLevelExceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string errorCode = "UNAUTHORIZED", string? message = null)
            : base(errorCode, message) { }
    }
}
