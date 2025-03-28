using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Exceptions.AccountException
{
    public class UsernameExistException : Exception
    {
        public UsernameExistException() : base($"Username already exists.")
        {
        }
    }
}
