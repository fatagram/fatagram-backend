using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.NotificationRepository.Interface;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendshipService(
        IFriendshipRepository friendshipRepository,
        IFriendRequestRepository friendRequestRepository,
        IUserRepository userRepository,
        IMapper mapper,
        IFriendRequestManager friendRequestManager,
        FriendshipValidator friendshipValidator,
        NotifyBuilder notifyBuilder,
        IUserNotificationRepository userNotificationRepository,
        ILogger<FriendshipService> logger
    ) : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository = friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository =
            friendRequestRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IFriendRequestManager _friendRequestManager = friendRequestManager;
        private readonly FriendshipValidator _validator = friendshipValidator;
        private readonly NotifyBuilder _notifyBuilder = notifyBuilder;
        private readonly IUserNotificationRepository _userNotificationRepository =
            userNotificationRepository;
        private readonly ILogger<FriendshipService> _logger = logger;

        public async Task<Result> SendFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            await _validator.ValidateSendFriendRequestAsync(senderId, receiverId);
            var res = await _friendRequestManager.CreateRequestAsync(senderId, receiverId);

            var dto = NotificationFactory.CreateNewFriendRequestNotification(
                receiverId,
                senderId,
                res.Data
            );
            await _notifyBuilder.WithPush().NotifyAsync(receiverId, new NotifyOptions(dto));

            return Result.Create();
        }

        public async Task<Result> AcceptFriendRequestAsync(Guid acceptorId, Guid requesterId)
        {
            await _validator.ValidateAcceptFriendRequestAsync(acceptorId, requesterId);
            await CreateFriendshipAsync(requesterId, acceptorId);
            await _friendRequestManager.DeleteRequestAsync(acceptorId, requesterId);

            // Build decorator chain: push + email (khi cần)
            var dto = NotificationFactory.CreateFriendRequestAcceptedNotification(
                requesterId,
                acceptorId
            );
            await _notifyBuilder
                .WithPush()
                // .WithEmail()  // uncomment khi muốn gửi email
                .NotifyAsync(requesterId, new NotifyOptions(dto));

            return Result.Create();
        }

        private async Task CreateFriendshipAsync(Guid user1Id, Guid user2Id)
        {
            var newFriendship = new Friendship { User1Id = user1Id, User2Id = user2Id };
            await _friendshipRepository.AddAsync(newFriendship);
        }

        public async Task<Result> RevokeFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            // Lấy friend request để lấy SourceId
            var friendRequest = (
                await _friendRequestRepository.GetAllAsync(
                    fr => fr.SenderId == senderId && fr.ReceiverId == receiverId,
                    fr => fr
                )
            ).FirstOrDefault();

            if (friendRequest != null)
            {
                // Query UserNotification theo SourceId thay vì ActorId + Type
                var userNotifications = await _userNotificationRepository.GetAllAsync<
                    UserNotification,
                    DateTime
                >(
                    un => un.UserId == receiverId && un.Notification.SourceId == friendRequest.Id,
                    un => un.Notification.CreatedAt,
                    false,
                    1,
                    null,
                    un => un.Include(un => un.Notification)
                );

                var userNotification = userNotifications.FirstOrDefault();
                if (userNotification != null)
                {
                    var cancelDto = NotificationFactory.CreateCancelNotification(
                        receiverId,
                        userNotification.Id
                    );
                    await _notifyBuilder
                        .WithPush()
                        .NotifyAsync(receiverId, new NotifyOptions(cancelDto, IsSave: false));

                    await _userNotificationRepository.DeleteAsync(userNotification);
                }

                await _friendRequestRepository.DeleteAsync(friendRequest);
            }

            return Result.Create();
        }

        public async Task<Result> DeclineFriendRequestAsync(Guid declinerId, Guid requesterId)
        {
            await _friendRequestManager.DeleteRequestAsync(declinerId, requesterId);
            return Result.Create();
        }

        public async Task<Result> UnfriendAsync(Guid userId, Guid friendId)
        {
            var friendship =
                (
                    await _friendshipRepository.GetAllAsync(
                        f =>
                            (f.User1Id == userId && f.User2Id == friendId)
                            || (f.User1Id == friendId && f.User2Id == userId),
                        s => s
                    )
                ).FirstOrDefault()
                ?? throw new BadRequestException(
                    new Shared.Common.Error("FRIENDSHIP_NOT_EXIST", "Friendship does not exist.")
                );
            await _friendshipRepository.DeleteAsync(friendship);

            return Result.Create();
        }

        public async Task<Result<GetFriendShipStatusDto>> GetFriendshipStatusAsync(
            Guid sourceId,
            Guid desId
        )
        {
            var friendship = (
                await _friendshipRepository.GetAllAsync(
                    fr =>
                        (fr.User1Id == sourceId && fr.User2Id == desId)
                        || (fr.User1Id == desId && fr.User2Id == sourceId),
                    s => s
                )
            ).FirstOrDefault();
            if (friendship != null)
                return Result<GetFriendShipStatusDto>.Create(
                    ResponseStatusCode.Success,
                    new GetFriendShipStatusDto { Status = FriendShipStatus.Friend }
                );

            var friendRequest = (
                await _friendRequestRepository.GetAllAsync(
                    fr => fr.SenderId == sourceId && fr.ReceiverId == desId,
                    s => s
                )
            ).FirstOrDefault();
            if (friendRequest != null)
                return Result<GetFriendShipStatusDto>.Create(
                    ResponseStatusCode.Success,
                    new GetFriendShipStatusDto { Status = FriendShipStatus.SentByMe }
                );

            var friendReceived = (
                await _friendRequestRepository.GetAllAsync(
                    fr => fr.SenderId == desId && fr.ReceiverId == sourceId,
                    s => s
                )
            ).FirstOrDefault();
            if (friendReceived != null)
                return Result<GetFriendShipStatusDto>.Create(
                    ResponseStatusCode.Success,
                    new GetFriendShipStatusDto { Status = FriendShipStatus.SentByThem }
                );

            return Result<GetFriendShipStatusDto>.Create(
                ResponseStatusCode.Success,
                new GetFriendShipStatusDto { Status = FriendShipStatus.None }
            );
        }

        public async Task<Result<int>> GetNumberOfFriendsAsync(Guid userId)
        {
            var user =
                await _userRepository.GetAsync(userId, u => u) ?? throw new UserNotFoundException();

            var count = await _friendshipRepository.CountAsync(u =>
                u.User1Id == userId || u.User2Id == userId
            );
            return Result<int>.Create(ResponseStatusCode.Success, count);
        }

        public async Task<CursorResult<DateTime, FriendRequestDto>> GetFriendRequestsAsync(
            Guid userId,
            CursorFilter<DateTime> filter
        )
        {
            _logger.LogInformation(
                "Getting friend requests for user {UserId} with filter {@Filter}",
                userId,
                filter
            );
            var result = await _friendRequestRepository.GetAllAsync(
                filter: fr => fr.ReceiverId == userId,
                selector: f => new FriendRequestDto
                {
                    SenderId = f.SenderId,
                    SenderAvatar = f.Sender.Avatar,
                    SenderName = f.Sender.FullName,
                    CreatedAt = f.CreatedAt,
                },
                orderBy: f => f.CreatedAt,
                orderDesc: filter.SortDesc ?? false,
                limit: filter.Limit,
                lastKey: filter.Cursor,
                include: q => q.Include(fr => fr.Sender)
            );

            _logger.LogInformation(
                "Retrieved {Count} friend requests for user {UserId}",
                result.Count(),
                userId
            );

            var hasNext = result.Count() == filter.Limit;
            var nextCursor = hasNext ? result.Last()?.CreatedAt : null;

            return CursorResult<DateTime, FriendRequestDto>.Create(
                result,
                nextCursor,
                hasNext,
                null,
                null
            );
        }

        public async Task<CursorResult<DateTime, FriendDto>> GetFriendsAsync(
            Guid userId,
            Guid targetId,
            CursorFilter<DateTime> filter
        )
        {
            var result = await _friendshipRepository.GetAllAsync(
                filter: f =>
                    (f.User1Id == targetId && f.User2Id == userId)
                    || (f.User1Id != userId && f.User2Id != targetId),
                selector: f => new FriendDto
                {
                    Id = f.User1Id == targetId ? f.User2Id : f.User1Id,
                    Avatar = f.User1Id == targetId ? f.User2.Avatar : f.User1.Avatar,
                    Name = (f.User1Id == targetId ? f.User2.FullName : f.User1.FullName)!,
                    CreatedAt = f.CreatedAt,
                },
                orderBy: f => f.CreatedAt,
                orderDesc: filter.SortDesc ?? false,
                limit: filter.Limit,
                lastKey: filter.Cursor,
                include: q => q.Include(f => f.User1).Include(f => f.User2)
            );

            var hasNext = result.Count() == filter.Limit;
            var nextCursor = hasNext ? result.Last()?.CreatedAt : null;

            return CursorResult<DateTime, FriendDto>.Create(
                result,
                nextCursor,
                hasNext,
                null,
                null
            );
        }
    }
}
