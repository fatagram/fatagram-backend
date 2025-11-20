using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class Email : BaseEntity
    {
        public string Address { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
    }
}
