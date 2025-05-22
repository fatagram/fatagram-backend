using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;

namespace Fatagram.Application.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException() : base("USER_NOT_FOUND", "User not found")
        { }
    }
}
