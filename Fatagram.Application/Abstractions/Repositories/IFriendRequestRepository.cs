using System;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IFriendRequestRepository : IBaseRepository<FriendRequest>
    {
        Task<bool> RequestExistsAsync(Guid senderId, Guid receiverId);
    }
}
