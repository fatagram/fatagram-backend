using System;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Common.Projections
{
    public class FriendProjection
    {
        public User User { get; set; } = null!;
        public bool IsFriend { get; set; }
    }
}
