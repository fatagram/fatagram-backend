using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendshipNotificationStrategy : IFriendshipNotificationStrategy
    {
        public Task NotifyFriendRequestAcceptedAsync(Guid acceptorId, Guid requesterId)
        {
            return Task.CompletedTask;
        }

        public Task NotifyFriendRequestSentAsync(Guid senderId, Guid receiverId)
        {
            return Task.CompletedTask;
        }
    }
}
