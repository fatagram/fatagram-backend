using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class CursorPagedResult<TCursor, TItem>
    {
        public bool IsSuccess { get; set; }
        public IEnumerable<TItem> Items { get; set; } = new List<TItem>();
        public int Total { get; set; }
        public TCursor? NextCursor { get; set; }
        public string? Message { get; }
        public ResponseStatusCode Code { get; set; }

        /// <summary>
        /// The error code
        /// </summary>
        public string? ErrorCode { get; }

        /// <summary>
        /// The error message
        /// </summary>
        public string? ErrorMessage { get; }
        public Dictionary<string, object> ExtraInfo { get; set; } =
            new Dictionary<string, object>();

        private CursorPagedResult(
            bool isSuccess,
            IEnumerable<TItem> items,
            TCursor? nextCursor,
            string? message = null,
            string? errorCode = null,
            string? errorMessage = null,
            Dictionary<string, object>? extraInfo = null
        )
        {
            IsSuccess = isSuccess;
            Items = items;
            NextCursor = nextCursor;
            Message = message;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
            if (extraInfo != null)
            {
                ExtraInfo = extraInfo;
            }
        }

        public static CursorPagedResult<TCursor, TItem> Success(
            IEnumerable<TItem> items,
            TCursor? nextCursor,
            string? message = null,
            Dictionary<string, object>? extraInfo = null
        )
        {
            return new CursorPagedResult<TCursor, TItem>(
                true,
                items,
                nextCursor,
                message,
                null,
                null,
                extraInfo
            );
        }

        public static CursorPagedResult<TCursor, TItem> Failure(
            string? errorCode = null,
            string? errorMessage = null,
            Dictionary<string, object>? extraInfo = null
        )
        {
            return new CursorPagedResult<TCursor, TItem>(
                false,
                new List<TItem>(),
                default,
                null,
                errorCode,
                errorMessage,
                extraInfo
            );
        }
    }
}
