using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class ValidateException(List<Error>? errors)
        : BadRequestException(new Error("VALIDATION_FAILED", "Validation failed."), errors) { }
}
