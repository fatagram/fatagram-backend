using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class GenerateTokenException : AppException
    {
        public GenerateTokenException()
            : base("GENERATE_TOKEN_FAILED", "Failed to generate token.") { }
    }
}
