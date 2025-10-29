using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class PagedResult<T>
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Total { get; set; } = 0;
        // public int Page { get; set; } = 1;
        // public int PageSize { get; set; } = 10;
        public string? Message { get; }
        public ResponseStatusCode Code { get; set; } = ResponseStatusCode.Success;
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }
        public Dictionary<string, object> ExtraInfo { get; set; } =
            new Dictionary<string, object>();

        private PagedResult(
            bool isSuccess,
            IEnumerable<T> items,
            int totalCount,
            // int page,
            // int pageSize,
            string? message,
            string? errorCode,
            string? errorMessage,
            Dictionary<string, object> extraInfo
        )
        {
            IsSuccess = isSuccess;
            Items = items;
            Total = totalCount;
            // Page = page;
            // PageSize = pageSize;
            Message = message;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            ExtraInfo = extraInfo;
        }

        public static PagedResult<T> Success(
            IEnumerable<T> items,
            int totalCount,
            // int page,
            // int pageSize,
            string? message = null,
            Dictionary<string, object>? extraInfo = null
        )
        {
            return new PagedResult<T>(
                true,
                items,
                totalCount,
                // page,
                // pageSize,
                message,
                null,
                null,
                extraInfo ?? new Dictionary<string, object>()
            );
        }

        public static PagedResult<T> Failure(
            string errorCode,
            string errorMessage,
            string? message = null,
            Dictionary<string, object>? extraInfo = null
        )
        {
            return new PagedResult<T>(
                false,
                new List<T>(),
                0,
                // 1,
                // 10,
                message,
                errorCode,
                errorMessage,
                extraInfo ?? new Dictionary<string, object>()
            );
        }
    }
}
