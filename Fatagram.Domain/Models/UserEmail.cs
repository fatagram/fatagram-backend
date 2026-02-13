using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class UserEmail : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid EmailId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsVerified { get; set; }
        public User User { get; set; } = null!;
        public Email Email { get; set; } = null!;
    }
}
