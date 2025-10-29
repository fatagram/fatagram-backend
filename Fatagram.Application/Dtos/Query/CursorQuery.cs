using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Query
{
    public class CursorQuery<TCursor> : BaseQuery
    {
        public TCursor? Cursor { get; set; }
        public int Limit { get; set; } = 10;
    }
}
