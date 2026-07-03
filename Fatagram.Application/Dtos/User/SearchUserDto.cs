using System;
using Fatagram.Domain.Enums;

namespace Fatagram.Application.Dtos.User
{
    public record SearchUserDto
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? UrlName { get; set; }
        public string? Avatar { get; set; }
        public FriendShipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
