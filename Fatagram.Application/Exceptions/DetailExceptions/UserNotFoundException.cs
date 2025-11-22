using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class UserNotFoundException()
        : NotFoundException(new Error("USER_NOT_FOUND", "User not found")) { }
}
