using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Domain.Models
{
    public class FriendRequest : BaseEntity
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
    }
}
