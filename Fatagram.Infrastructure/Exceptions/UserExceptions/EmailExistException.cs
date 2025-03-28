using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Exceptions.UserExceptions
{
    public class EmailExistException : Exception
    {
        public EmailExistException() : base($"Email already exists.")
        {
        }
    }
}
