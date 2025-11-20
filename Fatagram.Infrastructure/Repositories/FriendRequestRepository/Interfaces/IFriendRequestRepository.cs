using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces
{
    public interface IFriendRequestRepository : IBaseRepository<FriendRequest>
    {
        Task<bool> RequestExistsAsync(Guid senderId, Guid receiverId);
    }
}
