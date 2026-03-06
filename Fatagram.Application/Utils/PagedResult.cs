using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int PageSize { get; set; } = 0;
        public int Page { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int Total { get; set; } = 0;
        public Dictionary<string, object>? ExtraInfo { get; set; }

        private PagedResult(
            IEnumerable<T> items,
            int page,
            int pageSize,
            int total,
            int totalPages,
            Dictionary<string, object>? extraInfo
        )
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            Total = total;
            TotalPages = totalPages;
            ExtraInfo = extraInfo;
        }
    }
}
