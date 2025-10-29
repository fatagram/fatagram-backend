using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Query
{
    public class PagedQuery : BaseQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
