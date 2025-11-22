using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendshipService(
        IFriendshipRepository friendshipRepository,
        IFriendRequestRepository friendRequestRepository,
        IUserRepository userRepository,
        IMapper mapper,
        INotificationService notificationService
    ) : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository = friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository =
            friendRequestRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly INotificationService _notificationService = notificationService;

        public async Task<Result<object>> SendAddFriendAsync(Guid senderId, Guid receiverId)
        {
            // Check if two users are friends
            if (await _friendshipRepository.AreFriendsAsync(senderId, receiverId))
            {
                throw new AppException(
                    new("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.")
                );
            }

            // Check if a request already exists
            if (await _friendRequestRepository.RequestExistsAsync(senderId, receiverId))
            {
                throw new AppException(
                    new("REQUEST_ALREADY_EXIST", "Friend request already exists.")
                );
            }

            // Check if the sender exists
            var sender = await _userRepository.GetAsync(senderId, s => s.Id);
            if (sender == Guid.Empty)
            {
                throw new UserNotFoundException();
            }

            // Create a new friend request
            var newFriendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                CreatedAt = DateTime.UtcNow,
            };

            // Create notification for the receiver
            var notificationDto = NotificationFactory.CreateNewFriendRequestNotification(
                receiverId,
                senderId
            );

            await _notificationService.CreateNotificationAsync(receiverId, notificationDto);
            await _friendRequestRepository.AddAsync(newFriendRequest);
            return Result<object>.Create();
        }

        public async Task<Result<object>> AcceptAddFriendAsync(Guid acceptorId, Guid requesterId)
        {
            // Check if the two users are already friends
            if (await _friendshipRepository.AreFriendsAsync(acceptorId, requesterId))
            {
                throw new AppException(
                    new("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.")
                );
            }

            var request = await _friendRequestRepository.GetAllAsync<FriendRequest, Guid>(
                fr =>
                    fr.SenderId == requesterId && fr.ReceiverId == acceptorId
                    || fr.SenderId == acceptorId && fr.ReceiverId == requesterId,
                s => s
            );
            if (!await _friendRequestRepository.RequestExistsAsync(requesterId, acceptorId))
            {
                throw new AppException(new("REQUEST_NOT_EXIST", "Friend request does not exist."));
            }

            // Check if the acceptor exists
            var _acceptorId = await _userRepository.GetAsync(acceptorId, s => s.Id);
            if (_acceptorId == Guid.Empty)
            {
                throw new UserNotFoundException();
            }
            var newFrienship = new Friendship
            {
                User1Id = requesterId,
                User2Id = acceptorId,
                CreatedAt = DateTime.UtcNow,
            };

            await _friendshipRepository.AddAsync(newFrienship);
            await _friendRequestRepository.DeleteAsync(_acceptorId);

            // Notification handler
            await _notificationService.DeleteNotificationsAsync(
                acceptorId,
                requesterId,
                NotificationType.NewFriendRequest,
                isSendCancel: false
            );

            // Send notification to the requester
            var requesterNotification = NotificationFactory.CreateFriendRequestAcceptedNotification(
                requesterId,
                acceptorId
            );
            await _notificationService.CreateNotificationAsync(requesterId, requesterNotification);

            return Result<object>.Create();
        }

        public async Task<Result<object>> CancelAddFriendAsync(Guid senderId, Guid receiverId)
        {
            var friendRequest =
                (
                    await _friendRequestRepository.GetAllAsync(
                        fr => fr.SenderId == senderId && fr.ReceiverId == receiverId,
                        s => s
                    )
                ).FirstOrDefault()
                ?? throw new AppException(
                    new("REQUEST_NOT_EXIST", "Friend request does not exist.")
                );
            await _friendRequestRepository.DeleteAsync(friendRequest.Id);

            await _notificationService.DeleteNotificationsAsync(
                receiverId,
                senderId,
                NotificationType.NewFriendRequest
            );

            return Result<object>.Create();
        }

        public async Task<Result<object>> DeclineAddFriendRequestAsync(
            Guid declinerId,
            Guid requesterId
        )
        {
            var friendRequest =
                (
                    await _friendRequestRepository.GetAllAsync(
                        fr => fr.SenderId == requesterId && fr.ReceiverId == declinerId,
                        s => s
                    )
                ).FirstOrDefault()
                ?? throw new AppException(
                    new("REQUEST_NOT_EXIST", "Friend request does not exist.")
                );

            await _friendRequestRepository.DeleteAsync(friendRequest.Id);
            await _notificationService.DeleteNotificationsAsync(
                declinerId,
                requesterId,
                NotificationType.NewFriendRequest
            );

            return Result<object>.Create();
        }

        public async Task<Result<object>> UnfriendAsync(Guid userId, Guid friendId)
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
                ?? throw new AppException(
                    new("FRIENDSHIP_NOT_EXIST", "Friendship does not exist.")
                );

            await _friendshipRepository.DeleteAsync(friendship.Id);
            return Result<object>.Create();
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

            var count = await _friendshipRepository
                .GetAllAsync(f => f.User1Id == userId || f.User2Id == userId, s => s.Id)
                .ContinueWith(t => t.Result.Count);
            return Result<int>.Create(ResponseStatusCode.Success, count);
        }

        public async Task<PagedResult<FriendRequestDto>> GetFriendRequestsAsync(
            Guid userId,
            CursorFilter<Guid> filter
        )
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<FriendDto>> GetFriendsAsync(
            Guid userId,
            Guid targetId,
            CursorFilter<Guid> filter
        )
        {
            throw new NotImplementedException();
        }
    }
}
