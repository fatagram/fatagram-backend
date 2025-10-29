using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class ValidateException : BadRequestException
    {
        public ValidateException(List<string>? errorMessages)
            : base("VALIDATION_ERROR", "Validation failed", errorMessages) { }
    }
}
