using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class CursorResult<TCursor, TData> : Result<IEnumerable<TData>>
    {
        public int Total { get; set; }
        public TCursor? NextCursor { get; set; }
        public bool HasNextCursor { get; set; }
        public Dictionary<string, object> ExtraInfo { get; set; } =
            new Dictionary<string, object>();

        private CursorResult(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            int total,
            bool hasNextCursor,
            Dictionary<string, object>? extraInfo = null,
            string? message = null
        )
            : base(ResponseStatusCode.Success, data, message)
        {
            Data = data;
            NextCursor = nextCursor;
            Total = total;
            HasNextCursor = hasNextCursor;
        }

        public static CursorResult<TCursor, TData> Create(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            int total,
            bool hasNextCursor,
            Dictionary<string, object>? extraInfo = null,
            string? message = null
        )
        {
            return new CursorResult<TCursor, TData>(
                data,
                nextCursor,
                total,
                hasNextCursor,
                extraInfo,
                message
            );
        }
    }
}
