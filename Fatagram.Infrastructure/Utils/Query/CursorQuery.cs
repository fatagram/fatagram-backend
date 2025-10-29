using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Utils.Query
{
    public class CursorQuery<T> : BaseQuery
    {
        public T? Cursor { get; set; }
        public int Limit { get; set; } = 10;
    }
}
