using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class DataNullException : AppException
    {
        public DataNullException(string errorCode = "DATA_NULL", string? message = null)
            : base(errorCode, null, message)
        { }
    }
}
