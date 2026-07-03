using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public interface IFriendshipService
    {
        Task<Result> SendFriendRequestAsync(Guid senderId, Guid receiverId);

        Task<Result> AcceptFriendRequestAsync(Guid acceptorId, Guid requesterId);

        Task<Result> RevokeFriendRequestAsync(Guid receiverId, Guid senderId);

        Task<Result> UnfriendAsync(Guid userId, Guid friendId);

        Task<Result> DeclineFriendRequestAsync(Guid declinerId, Guid requesterId);
        Task<Result<GetFriendShipStatusDto>> GetFriendshipStatusAsync(Guid sourceId, Guid desId);

        Task<Result<int>> GetNumberOfFriendsAsync(Guid userId);

        Task<Result<CursorResult<FriendRequestDto, DateTime>>> GetFriendRequestsAsync(
            Guid userId,
            CursorFilter<DateTime> filter
        );

        Task<Result<CursorResult<FriendDto, DateTime>>> GetFriendsAsync(
            Guid userId,
            Guid targetId,
            CursorFilter<DateTime> filter
        );
    }
}
