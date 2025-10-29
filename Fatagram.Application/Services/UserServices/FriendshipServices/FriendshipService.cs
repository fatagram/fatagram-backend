using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.NotificationServices;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums.NotificationServices;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Infrastructure.Utils.Query;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public FriendshipService(
            IFriendshipRepository friendshipRepository,
            IFriendRequestRepository friendRequestRepository,
            IUserRepository userRepository,
            IMapper mapper,
            INotificationService notificationService
        )
        {
            _friendshipRepository = friendshipRepository;
            _friendRequestRepository = friendRequestRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<Result<object>> SendAddFriendAsync(Guid senderId, Guid receiverId)
        {
            // Check if two users are friends
            if ((await _friendshipRepository.GetAsync(senderId, receiverId)) != null)
            {
                throw new AppException("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.");
            }

            // Check if a request already exists
            if (
                (await _friendRequestRepository.GetAsync(senderId, receiverId)) != null
                || (await _friendRequestRepository.GetAsync(receiverId, senderId)) != null
            )
            {
                throw new AppException("REQUEST_ALREADY_EXIST", "Friend request already exists.");
            }

            // Check if the sender exists
            var sender = await _userRepository.GetAsync(senderId.ToString());
            if (sender == null)
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
            var friendshipExist = await _friendshipRepository.GetAsync(acceptorId, requesterId);
            if (friendshipExist != null)
            {
                throw new AppException("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.");
            }

            // Check if the friend request exists
            var friendRequest = await _friendRequestRepository.GetAsync(requesterId, acceptorId);
            if (friendRequest == null)
            {
                throw new AppException("REQUEST_NOT_EXIST", "Friend request does not exist.");
            }

            // Check if the acceptor exists
            var acceptor = await _userRepository.GetAsync(acceptorId.ToString());
            if (acceptor == null)
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
            await _friendRequestRepository.DeleteAsync(friendRequest);

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
            var friendRequest = await _friendRequestRepository.GetAsync(senderId, receiverId);
            if (friendRequest == null)
            {
                throw new AppException("REQUEST_NOT_EXIST", "Friend request does not exist.");
            }
            await _friendRequestRepository.DeleteAsync(friendRequest);

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
            var friendRequest = await _friendRequestRepository.GetAsync(requesterId, declinerId);
            if (friendRequest == null)
            {
                throw new AppException("REQUEST_NOT_EXIST", "Friend request does not exist.");
            }

            await _friendRequestRepository.DeleteAsync(friendRequest);
            await _notificationService.DeleteNotificationsAsync(
                declinerId,
                requesterId,
                NotificationType.NewFriendRequest
            );

            return Result<object>.Create();
        }

        public async Task<Result<object>> UnfriendAsync(Guid userId, Guid friendId)
        {
            var friendship = await _friendshipRepository.GetAsync(userId, friendId);
            if (friendship == null)
            {
                throw new AppException("FRIENDSHIP_NOT_EXIST", "Friendship does not exist.");
            }

            await _friendshipRepository.DeleteAsync(friendship);
            return Result<object>.Create();
        }

        public async Task<Result<GetFriendShipStatusDto>> GetFriendshipStatusAsync(
            Guid sourceId,
            Guid desId
        )
        {
            var friendship = await _friendshipRepository.GetAsync(sourceId, desId);
            if (friendship != null)
                return Result<GetFriendShipStatusDto>.Create(
                    ResponseStatusCode.Success,
                    new GetFriendShipStatusDto { Status = FriendShipStatus.Friend }
                );

            var friendRequest = await _friendRequestRepository.GetAsync(sourceId, desId);
            if (friendRequest != null)
                return Result<GetFriendShipStatusDto>.Create(
                    ResponseStatusCode.Success,
                    new GetFriendShipStatusDto { Status = FriendShipStatus.SentByMe }
                );

            var friendReceived = await _friendRequestRepository.GetAsync(desId, sourceId);
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
            var user = await _userRepository.GetAsync(userId.ToString());
            if (user == null)
                throw new UserNotFoundException();

            var count = await _friendshipRepository.CountAsync(userId);
            return Result<int>.Create(ResponseStatusCode.Success, count);
        }

        public async Task<PagedResult<FriendRequestDto>> GetFriendRequestsAsync(
            Guid userId,
            PagedFilter filter
        )
        {
            var data = await _friendRequestRepository.GetFriendRequestsAsync(
                userId,
                filter.Page,
                filter.PageSize
            );
            var friendRequests = _mapper.Map<List<FriendRequestDto>>(data.requests);
            return PagedResult<FriendRequestDto>.Create(
                friendRequests,
                filter.Page,
                filter.PageSize,
                data.total,
                (int)Math.Ceiling((double)data.total / filter.PageSize)
            );
        }

        public async Task<PagedResult<FriendDto>> GetFriendsAsync(
            Guid userId,
            Guid targetId,
            PagedFilter filter
        )
        {
            var data = await _friendshipRepository.GetFriendsOfUserAsync(
                userId,
                targetId,
                filter.Keyword,
                filter.Page,
                filter.PageSize
            );
            var friends = _mapper.Map<IEnumerable<FriendDto>>(data.friends);
            return PagedResult<FriendDto>.Create(
                friends,
                filter.Page,
                filter.PageSize,
                data.total,
                (int)Math.Ceiling((double)data.total / filter.PageSize)
            );
        }
    }
}
