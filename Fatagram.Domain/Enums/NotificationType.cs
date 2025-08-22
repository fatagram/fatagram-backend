using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Enums.NotificationServices
{
    public enum NotificationType
    {
        NewFriendRequest,
        FriendRequestAccepted,
        FriendRequestCanceled,
        System,
        CancelNotification,
    }
}