using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.UserServices.FriendshipServices.Interface
{
    public interface IFriendshipNotificationStrategy
    {
        Task NotifyFriendRequestSentAsync(Guid senderId, Guid receiverId);
        Task NotifyFriendRequestAcceptedAsync(Guid acceptorId, Guid requesterId);
    }
}
