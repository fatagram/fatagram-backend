using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class UserNotFoundException : AppException
    {
        public UserNotFoundException() : base("USER_NOT_FOUND", "User not found")
        { }
    }
}
