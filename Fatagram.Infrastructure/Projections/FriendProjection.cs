using Fatagram.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Projections
{
    public class FriendProjection
    {
        public User User { get; set; } = null!;
        public bool IsFriend { get; set; }
    }
}
