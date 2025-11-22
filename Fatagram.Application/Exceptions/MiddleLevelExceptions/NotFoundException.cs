using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.MiddleLevelExceptions
{
    public class NotFoundException(Error error) : AppException(error) { }
}
