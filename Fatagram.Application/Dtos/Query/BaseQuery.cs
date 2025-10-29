using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos
{
    public class BaseQuery
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool? SortDesc { get; set; } = false;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
