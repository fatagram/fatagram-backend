using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Utils;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;

namespace Fatagram.Application.Services.UserServices.FriendshipServices.Interface
{
    public interface IFriendRequestManager
    {
        Task<Result> CreateRequestAsync(Guid senderId, Guid receiverId);

        Task<Result> DeleteRequestAsync(Guid receiverId, Guid senderId);
    }
}
