using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class GenerateTokenException()
        : BadRequestException(new Error("GENERATE_TOKEN_FAILED", "Failed to generate token.")) { }
}
