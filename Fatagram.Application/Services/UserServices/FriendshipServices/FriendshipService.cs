using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.FriendRequestRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.FriendshipRepository.Interfaces;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
using Fatagram.Shared.Utils;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendshipService : IFriendshipService
    {
        private readonly IFriendshipRepository _friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public FriendshipService(
            IFriendshipRepository friendshipRepository,
            IFriendRequestRepository friendRequestRepository,
            IUserRepository userRepository,
            IMapper mapper
        )
        {
            _friendshipRepository = friendshipRepository;
            _friendRequestRepository = friendRequestRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<Result<object>> SendAddFriendAsync(Guid senderId, Guid receiverId)
        {
            if ((await _friendshipRepository.GetAsync(senderId, receiverId)) != null)
            {
                return Result<object>.BadRequest("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.");
            }

            if ((await _friendRequestRepository.GetAsync(senderId, receiverId)) != null
                    || (await _friendRequestRepository.GetAsync(receiverId, senderId)) != null)
            {
                return Result<object>.BadRequest("REQUEST_ALREADY_EXIST", "Friend request already exists.");
            }

            var newFriendRequest = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                CreatedAt = DateTime.UtcNow,
            };

            await _friendRequestRepository.AddAsync(newFriendRequest);
            return Result<object>.Success();
        }

        public async Task<Result<object>> AcceptAddFriendAsync(Guid acceptorId, Guid requesterId)
        {
            var friendshipExist = await _friendshipRepository.GetAsync(acceptorId, requesterId);
            if (friendshipExist != null)
            {
                return Result<object>.BadRequest("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.");
            }

            var friendRequest = await _friendRequestRepository.GetAsync(requesterId, acceptorId);
            if (friendRequest == null)
            {
                return Result<object>.BadRequest("REQUEST_NOT_EXIST", "Friend request does not exist.");
            }

            var newFrienship = new Friendship
            {
                User1Id = requesterId,
                User2Id = acceptorId,
                CreatedAt = DateTime.UtcNow,
            };

            await _friendshipRepository.AddAsync(newFrienship);
            await _friendRequestRepository.DeleteAsync(friendRequest);
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