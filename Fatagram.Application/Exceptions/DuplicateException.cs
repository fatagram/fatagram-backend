using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class DuplicateException : AppException
    {
        public DuplicateException(string? code = null, string? message = null) : base(code, message)
        { }
    }
}
