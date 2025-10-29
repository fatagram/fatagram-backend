using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.MiddleLevelExceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string errorCode, string message)
            : base(errorCode, message) { }
    }
}
