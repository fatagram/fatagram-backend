using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Utils
{
    public class CursorResult<TItem, TCursor>
        where TCursor : struct
    {
        public IEnumerable<TItem> Items { get; set; }
        public TCursor? NextCursor { get; set; }
        public bool HasNext { get; set; }
        public Dictionary<string, object> ExtraInfo { get; set; } =
            new Dictionary<string, object>();

        public CursorResult()
        {
            Items = Enumerable.Empty<TItem>();
            NextCursor = null;
            HasNext = false;
            ExtraInfo = new Dictionary<string, object>();
        }

        public CursorResult(
            IEnumerable<TItem> items,
            TCursor? nextCursor,
            bool hasNext,
            Dictionary<string, object>? extraInfo = null
        )
        {
            Items = items;
            NextCursor = nextCursor;
            HasNext = hasNext;
            ExtraInfo = extraInfo ?? new Dictionary<string, object>();
        }
    }
}
