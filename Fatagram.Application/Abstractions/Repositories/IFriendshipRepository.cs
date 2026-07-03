using System;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IFriendshipRepository : IBaseRepository<Friendship>
    {
        Task<bool> AreFriendsAsync(Guid user1Id, Guid user2Id);
    }
}
