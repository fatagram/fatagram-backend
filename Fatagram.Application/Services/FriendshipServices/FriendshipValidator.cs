using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Abstractions.Repositories;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendshipValidator(
        IFriendshipRepository friendshipRepository,
        IFriendRequestRepository friendRequestRepository,
        IUserRepository userRepository
    )
    {
        private readonly IFriendshipRepository _friendshipRepository = friendshipRepository;
        private readonly IFriendRequestRepository _friendRequestRepository =
            friendRequestRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task ValidateSendFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            await EnsureNotAlreadyFriendsAsync(senderId, receiverId);
            await EnsureRequestNotExistsAsync(senderId, receiverId);
            await EnsureUserExistsAsync(senderId);
        }

        public async Task ValidateAcceptFriendRequestAsync(Guid acceptorId, Guid requesterId)
        {
            await EnsureNotAlreadyFriendsAsync(acceptorId, requesterId);
            await EnsureRequestExistsAsync(requesterId, acceptorId);
            await EnsureUserExistsAsync(acceptorId);
        }

        public async Task EnsureNotAlreadyFriendsAsync(Guid user1Id, Guid user2Id)
        {
            if (await _friendshipRepository.AreFriendsAsync(user1Id, user2Id))
            {
                throw new AppException(
                    new("FRIENDSHIP_ALREADY_EXIST", "Friendship already exists.")
                );
            }
        }

        public async Task EnsureRequestExistsAsync(Guid senderId, Guid receiverId)
        {
            if (!await _friendRequestRepository.RequestExistsAsync(senderId, receiverId))
            {
                throw new AppException(new("REQUEST_NOT_EXIST", "Friend request does not exist."));
            }
        }

        private async Task EnsureRequestNotExistsAsync(Guid senderId, Guid receiverId)
        {
            if (await _friendRequestRepository.RequestExistsAsync(senderId, receiverId))
            {
                throw new BadRequestException(
                    new("REQUEST_ALREADY_EXIST", "Friend request already exists.")
                );
            }
        }

        private async Task EnsureUserExistsAsync(Guid userId)
        {
            var user = await _userRepository.GetAsync(userId, u => u.Id);
            if (user == Guid.Empty)
            {
                throw new UserNotFoundException();
            }
        }
    }
}
