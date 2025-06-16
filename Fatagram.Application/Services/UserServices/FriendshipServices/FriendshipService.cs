using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Application.Utils;
using Fatagram.Application.Services.NotificationServices.Interface;
using Fatagram.Application.Services.NotificationServices;

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
                return Result<object>.BadRequest("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.");
            }

            // Check if a request already exists
            if ((await _friendRequestRepository.GetAsync(senderId, receiverId)) != null
                    || (await _friendRequestRepository.GetAsync(receiverId, senderId)) != null)
            {
                return Result<object>.BadRequest("REQUEST_ALREADY_EXIST", "Friend request already exists.");
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
                receiverId.ToString(),
                senderId.ToString(),
                $"/{senderId}"
            );

            await _notificationService.CreateNotificationAsync(notificationDto);
            await _friendRequestRepository.AddAsync(newFriendRequest);
            return Result<object>.Success();
        }

        public async Task<Result<object>> AcceptAddFriendAsync(Guid acceptorId, Guid requesterId)
        {
            // Check if the two users are already friends
            var friendshipExist = await _friendshipRepository.GetAsync(acceptorId, requesterId);
            if (friendshipExist != null)
            {
                return Result<object>.BadRequest("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.");
            }

            // Check if the friend request exists
            var friendRequest = await _friendRequestRepository.GetAsync(requesterId, acceptorId);
            if (friendRequest == null)
            {
                return Result<object>.BadRequest("REQUEST_NOT_EXIST", "Friend request does not exist.");
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
                acceptorId.ToString(),
                requesterId.ToString(),
                NotificationType.NewFriendRequest,
                isSendCancel: false
            );

            // Send notification to the requester
            var requesterNotification = NotificationFactory.CreateFriendRequestAcceptedNotification(
                requesterId.ToString(),
                acceptorId.ToString(),
                $"/{acceptorId}"
            );
            await _notificationService.CreateNotificationAsync(requesterNotification);

            return Result<object>.Success();
        }

        public async Task<Result<object>> CancelAddFriendAsync(Guid senderId, Guid receiverId)
        {
            var friendRequest = await _friendRequestRepository.GetAsync(senderId, receiverId);
            if (friendRequest == null)
            {
                return Result<object>.BadRequest("REQUEST_NOT_EXIST", "Friend request does not exist.");
            }
            await _friendRequestRepository.DeleteAsync(friendRequest);

            await _notificationService.DeleteNotificationsAsync(receiverId.ToString(), senderId.ToString(),
                NotificationType.NewFriendRequest);
  
            return Result<object>.Success();
        }

        public async Task<Result<object>> DeclineAddFriendRequestAsync(Guid declinerId, Guid requesterId)
        {
            var friendRequest = await _friendRequestRepository.GetAsync(requesterId, declinerId);
            if (friendRequest == null)
            {
                return Result<object>.BadRequest("REQUEST_NOT_EXIST", "Friend request does not exist.");
            }

            await _friendRequestRepository.DeleteAsync(friendRequest);
            await _notificationService.DeleteNotificationsAsync(declinerId.ToString(), requesterId.ToString(),
                NotificationType.NewFriendRequest);
            
            return Result<object>.Success();
        }

        public async Task<Result<object>> UnfriendAsync(Guid userId, Guid friendId)
        {
            var friendship = await _friendshipRepository.GetAsync(userId, friendId);
            if (friendship == null)
            {
                return Result<object>.BadRequest("FRIENDSHIP_NOT_EXIST", "Friendship does not exist.");
            }

            await _friendshipRepository.DeleteAsync(friendship);
            return Result<object>.Success();
        }

        public async Task<Result<GetFriendShipStatusDto>> GetFriendshipStatusAsync(Guid sourceId, Guid desId)
        {
            var friendship = await _friendshipRepository.GetAsync(sourceId, desId);
            if (friendship != null) return Result<GetFriendShipStatusDto>.Success(new GetFriendShipStatusDto
            {
                Status = FriendShipStatus.Friend
            });

            var friendRequest = await _friendRequestRepository.GetAsync(sourceId, desId);
            if (friendRequest != null) return Result<GetFriendShipStatusDto>.Success(new GetFriendShipStatusDto
            {
                Status = FriendShipStatus.SentByMe
            });

            var friendReceived = await _friendRequestRepository.GetAsync(desId, sourceId);
            if (friendReceived != null) return Result<GetFriendShipStatusDto>.Success(new GetFriendShipStatusDto
            {
                Status = FriendShipStatus.SentByThem
            });

            return Result<GetFriendShipStatusDto>.Success(new GetFriendShipStatusDto
            {
                Status = FriendShipStatus.None
            });
        }

        public async Task<Result<GetNumberOfFriendsDto>> GetNumberOfFriendsAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId.ToString());
            if (user == null)
                throw new UserNotFoundException();

            var count = await _friendshipRepository.CountAsync(userId);
            return Result<GetNumberOfFriendsDto>.Success(new GetNumberOfFriendsDto()
            {
                NumberOfFriends = count
            });
        }

        public async Task<Result<GetFriendRequestsDto>> GetFriendRequestsAsync(Guid userId, int page, int pageSize)
        {
            var data = await _friendRequestRepository.GetFriendRequestsAsync(userId, page, pageSize);
            var total = data.total;

            var friendRequests = _mapper.Map<List<FriendRequestDto>>(data.requests);
            var ret = new GetFriendRequestsDto()
            {
                FriendRequests = friendRequests,
                Total = total
            };
            return Result<GetFriendRequestsDto>.Success(ret);
        }
    }
}