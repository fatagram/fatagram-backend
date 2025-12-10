using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class CursorResult<TCursor, TData> : Result<IEnumerable<TData>>
        where TCursor : struct
    {
        public TCursor? NextCursor { get; set; }
        public bool HasNext { get; set; }
        public Dictionary<string, object> ExtraInfo { get; set; } =
            new Dictionary<string, object>();

        private CursorResult(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            bool hasNext,
            Dictionary<string, object>? extraInfo = null,
            string? message = null
        )
            : base(ResponseStatusCode.Success, data, message)
        {
            Data = data;
            NextCursor = nextCursor;
            HasNext = hasNext;
        }

        public static CursorResult<TCursor, TData> Create(
            IEnumerable<TData> data,
            TCursor? nextCursor,
            bool hasNext,
            Dictionary<string, object>? extraInfo = null,
            string? message = null
        )
        {
            return new CursorResult<TCursor, TData>(data, nextCursor, hasNext, extraInfo, message);
        }
    }
}
