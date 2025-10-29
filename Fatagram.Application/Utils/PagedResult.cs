using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class PagedResult<T> : Result<IEnumerable<T>>
    {
        public int PageSize { get; set; } = 0;
        public int Page { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
        public int Total { get; set; } = 0;
        public Dictionary<string, object>? ExtraInfo { get; set; }

        private PagedResult(
            IEnumerable<T> data,
            int page,
            int pageSize,
            int total,
            int totalPages,
            string? message,
            Dictionary<string, object>? extraInfo
        )
            : base(ResponseStatusCode.Success, data, message)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            Total = total;
            TotalPages = totalPages;
            Message = message;
            ExtraInfo = extraInfo;
        }

        public static PagedResult<T> Create(
            IEnumerable<T> data,
            int page,
            int pageSize,
            int total,
            int totalPages,
            string? message = null,
            Dictionary<string, object>? extraInfo = null
        )
        {
            return new PagedResult<T>(data, page, pageSize, total, totalPages, message, extraInfo);
        }
    }
}
