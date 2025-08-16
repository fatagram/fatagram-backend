using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Dtos.User
{
    public class GetFriendsDto
    {
        public IEnumerable<FriendDto> Friends { get; set; } = new List<FriendDto>();
        public int Total { get; set; }
    }

    public class FriendDto
    {
        public Guid Id { get; set; }
        public string? Avatar { get; set; } = null;
        public string Name { get; set; } = null!;
        public string? UrlName { get; set; } = null;
        public bool IsFriend { get; set; }
    }
}
