using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.User;
using Fatagram.Shared.Utils;

namespace Fatagram.Application.Services.UserServices.FriendshipServices.Interface
{
    public interface IFriendshipService
    {
        Task<Result<object>> SendAddFriendAsync(Guid senderId, Guid receiverId);

        Task<Result<object>> AcceptAddFriendAsync(Guid acceptorId, Guid requesterId);

        Task<Result<object>> CancelAddFriendAsync(Guid receiverId, Guid senderId);

        Task<Result<object>> UnfriendAsync(Guid userId, Guid friendId);

        Task<Result<object>> DeclineAddFriendRequestAsync(Guid declinerId, Guid requesterId);

        Task<Result<GetFriendShipStatusDto>> GetFriendshipStatusAsync(Guid sourceId, Guid desId);

        Task<Result<GetNumberOfFriendsDto>> GetNumberOfFriendsAsync(Guid userId);

        Task<Result<GetFriendRequestsDto>> GetFriendRequestsAsync(Guid userId, int page, int pageSize);
    }
}