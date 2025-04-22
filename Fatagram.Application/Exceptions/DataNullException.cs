using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions
{
    public class DataNullException : AppException
    {
        public DataNullException() : base("DATA_NULL", "Data is null")
        { }
    }
}
