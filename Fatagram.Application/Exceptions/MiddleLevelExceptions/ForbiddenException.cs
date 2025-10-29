using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string errorCode, string message)
            : base(errorCode, message) { }
    }
}
