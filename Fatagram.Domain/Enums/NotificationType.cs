using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Domain.Enums.NotificationServices
{
    public enum ActorType
    {
        User,
        System,
        Group,
        Page,
    }

    public enum NotificationType
    {
        NewFriendRequest,
        FriendRequestAccepted,
        FriendRequestCanceled,
        System,
        CancelNotification,
    }

    public enum NotificationTargetType
    {
        User,
        Post,
        Comment,
    }
}
