using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Utils;
using Fatagram.Application.Abstractions.Repositories;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public interface IFriendRequestManager
    {
        Task<Result<Guid>> CreateRequestAsync(Guid senderId, Guid receiverId);

        Task<Result> DeleteRequestAsync(Guid receiverId, Guid senderId);
    }
}
