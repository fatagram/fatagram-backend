using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper.Configuration.Annotations;
using Fatagram.Application.Abstractions.Repositories;

namespace Fatagram.Application.Services.UserServices.FriendshipServices
{
    public class FriendRequestValidator(
        IFriendRequestRepository friendRequestRepo,
        IUserRepository userRepo,
        IFriendshipRepository friendshipRepo
    )
    {
        private readonly IFriendRequestRepository _friendRequestRepo = friendRequestRepo;
        private readonly IUserRepository _userRepo = userRepo;
        private readonly IFriendshipRepository _friendshipRepo = friendshipRepo;

        public async Task<bool> CanSendFriendRequestAsync(Guid senderId, Guid receiverId)
        {
            var sender = await _userRepo.GetAsync(senderId, u => u.Id);
            var receiver = await _userRepo.GetAsync(receiverId, u => u.Id);

            if (
                sender == Guid.Empty
                || receiver == Guid.Empty
                || (await _friendshipRepo.AreFriendsAsync(senderId, receiverId))
                || (await _friendRequestRepo.RequestExistsAsync(senderId, receiverId))
            )
            {
                return false;
            }
            return true;
        }

        public async Task<bool> CanAcceptRequestAsync(Guid acceptorId, Guid requesterId)
        {
            var friendRequest = await _friendRequestRepo.GetAllAsync(
                u => u.SenderId == requesterId && u.ReceiverId == acceptorId,
                u => u
            );
            return friendRequest != null;
        }

        public async Task<bool> CanRevokeRequestAsync(Guid receiverId, Guid senderId)
        {
            var friendRequest = await _friendRequestRepo.GetAllAsync(
                u => u.SenderId == senderId && u.ReceiverId == receiverId,
                u => u
            );
            return friendRequest != null;
        }

        public async Task<bool> CanDeclineRequestAsync(Guid declinerId, Guid requesterId)
        {
            var friendRequest = await _friendRequestRepo.GetAllAsync(
                u => u.SenderId == requesterId && u.ReceiverId == declinerId,
                u => u
            );
            return friendRequest != null;
        }
    }
}
