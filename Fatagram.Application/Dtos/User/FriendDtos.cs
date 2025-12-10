using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Dtos.User
{
    public enum FriendShipStatus
    {
        None,
        Friend,
        SentByMe,
        SentByThem,
    }

    public class GetFriendShipStatusDto
    {
        public FriendShipStatus? Status { get; set; }
    }

    public class FriendRequestDto
    {
        public Guid SenderId { get; set; }
        public string? SenderUrlName { get; set; } = null;
        public string? SenderAvatar { get; set; }
        public string? SenderName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }
}
