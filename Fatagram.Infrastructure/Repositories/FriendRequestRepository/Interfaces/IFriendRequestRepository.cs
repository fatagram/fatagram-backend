using Fatagram.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces
{
    public interface IFriendRequestRepository
    {
        Task AddAsync(FriendRequest friendRequest);
        Task DeleteAsync(FriendRequest friendRequest);
        Task<FriendRequest?> GetAsync(Guid senderId, Guid receiverId);
        Task<List<FriendRequest>> GetFriendRequestsAsync(Guid userId, int page, int pageSize);

    }
}
