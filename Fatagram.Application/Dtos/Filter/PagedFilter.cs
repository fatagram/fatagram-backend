using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.Filter
{
    public class PagedFilter : BaseFilter
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
