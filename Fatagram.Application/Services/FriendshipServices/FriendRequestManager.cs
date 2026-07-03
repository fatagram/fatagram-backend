using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.UserServices.FriendshipServices;
using Fatagram.Application.Utils;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendRequestManager(IFriendRequestRepository friendRequestRepository)
        : IFriendRequestManager
    {
        private readonly IFriendRequestRepository _friendRequestRepository =
            friendRequestRepository;

        public async Task<Result<Guid>> CreateRequestAsync(Guid senderId, Guid receiverId)
        {
            var request = await _friendRequestRepository.AddAsync(
                new Domain.Models.FriendRequest
                {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    CreatedAt = DateTime.UtcNow,
                }
            );
            return Result<Guid>.Create(ResponseStatusCode.Success, request.Id);
        }

        public async Task<Result> DeleteRequestAsync(Guid receiverId, Guid senderId)
        {
            var friendRequest = (
                await _friendRequestRepository.GetAllAsync(
                    u => u.SenderId == senderId && u.ReceiverId == receiverId,
                    u => u
                )
            ).FirstOrDefault();
            if (friendRequest == null)
            {
                throw new BadRequestException(
                    new Error("FRIEND_REQUEST_NOT_FOUND", "Friend request does not exist")
                );
            }

            await _friendRequestRepository.DeleteAsync(friendRequest);
            return Result.Create();
        }
    }
}
