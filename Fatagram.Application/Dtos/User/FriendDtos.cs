using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public enum FriendShipStatus
    {
        None,
        Friend,
        SentByMe,
        SentByThem
    }

    public class GetFriendShipStatusDto
    {
        public FriendShipStatus? Status { get; set; }
    }

    public class GetNumberOfFriendsDto
    {
        public int NumberOfFriends { get; set; }
    }
}
