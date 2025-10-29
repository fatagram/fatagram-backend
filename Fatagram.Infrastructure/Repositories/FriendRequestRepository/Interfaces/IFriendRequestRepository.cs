using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;

namespace Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces
{
    public interface IFriendRequestRepository
    {
        Task AddAsync(FriendRequest friendRequest);
        Task DeleteAsync(FriendRequest friendRequest);
        Task<FriendRequest?> GetAsync(Guid senderId, Guid receiverId);
        Task<(List<FriendRequest> requests, int total)> GetFriendRequestsAsync(
            Guid userId,
            int page,
            int pageSize
        );
    }
}
