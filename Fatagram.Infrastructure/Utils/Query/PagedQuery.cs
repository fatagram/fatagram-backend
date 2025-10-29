using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Utils.Query
{
    public class PagedQuery<T> : BaseQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
