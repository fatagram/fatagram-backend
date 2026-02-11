using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Controllers.V1;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1.UserControllers
{
    public class FriendshipController(
        IFriendshipService friendshipService,
        ILogger<FriendshipController> logger
    ) : BaseApiController
    {
        private readonly IFriendshipService _friendshipService = friendshipService;
        private readonly ILogger<FriendshipController> _logger = logger;

        /// <summary>
        /// Send a friend request to another user.
        /// </summary>
        /// <param name="receiverId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("add/{receiverId}")]
        public async Task<IActionResult> AddFriend([FromRoute] string receiverId)
        {
            var userId = GetCurrentUserId();
            var result = await _friendshipService.SendFriendRequestAsync(
                userId,
                receiverId.ToGuid()
            );
            return result.ToActionResult();
        }

        /// <summary>
        /// Accept a friend request from another user.
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("accept/{senderId}")]
        public async Task<IActionResult> AcceptFriend([FromRoute] string senderId)
        {
            var userId = GetCurrentUserId();
            var result = await _friendshipService.AcceptFriendRequestAsync(
                userId,
                senderId.ToGuid()
            );
            return result.ToActionResult();
        }

        /// <summary>
        /// Get the friendship status between the authenticated user and the target user.
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpGet("status/{targetId}")]
        public async Task<IActionResult> GetFriendshipStatus([FromRoute] string targetId)
        {
            var userId = GetCurrentUserId();
            var res = await _friendshipService.GetFriendshipStatusAsync(userId, targetId.ToGuid());
            return res.ToActionResult();
        }

        /// <summary>
        /// Cancel a friend request sent by the authenticated user to another user.
        /// </summary>
        /// <param name="senderId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpDelete("cancel/{senderId}")]
        public async Task<IActionResult> CancelAddFriendRequest([FromRoute] string senderId)
        {
            var userId = GetCurrentUserId();
            var res = await _friendshipService.RevokeFriendRequestAsync(userId, senderId.ToGuid());
            return res.ToActionResult();
        }

        /// <summary>
        /// Decline a friend request sent by another user to the authenticated user.
        /// </summary>
        /// <param name="requesterId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpDelete("decline/{requesterId}")]
        public async Task<IActionResult> DeclineAddFriendRequest([FromRoute] string requesterId)
        {
            var userId = GetCurrentUserId();
            var res = await _friendshipService.DeclineFriendRequestAsync(
                userId,
                requesterId.ToGuid()
            );
            return res.ToActionResult();
        }

        /// <summary>
        /// Unfriend a user, removing the friendship between the authenticated user and the specified friend.
        /// </summary>
        /// <param name="friendId"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpDelete("unfriend/{friendId}")]
        public async Task<IActionResult> Unfriend([FromRoute] string friendId)
        {
            var userId = GetCurrentUserId();
            var res = await _friendshipService.UnfriendAsync(userId, friendId.ToGuid());
            return res.ToActionResult();
        }

        /// <summary>
        /// Get the number of friends for a specific user by their ID.
        /// </summary>
        /// <param name="targetId"></param>
        /// <returns></returns>
        [HttpGet("count/{targetId}")]
        public async Task<IActionResult> GetNumberOfFriends(string targetId)
        {
            var res = await _friendshipService.GetNumberOfFriendsAsync(targetId.ToGuid());
            return res.ToActionResult();
        }

        /// <summary>
        /// Get a paginated list of friend requests for the authenticated user.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedException"></exception>
        [Authorize]
        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequests(
            [FromQuery] CursorFilter<DateTime> filter
        )
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation(
                "Getting friend requests for user {UserId} with filter {@Filter}",
                userId,
                filter
            );
            var res = await _friendshipService.GetFriendRequestsAsync(userId, filter);
            return res.ToActionResult();
        }

        /// <summary>
        /// Get a list of friends for a specific user, with optional keyword search and pagination.
        /// </summary>
        /// <param name="targetId"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("friends/{targetId}")]
        public async Task<IActionResult> GetFriends(
            string targetId,
            [FromQuery] CursorFilter<DateTime> filter
        )
        {
            var userId = GetCurrentUserId();
            var res = await _friendshipService.GetFriendsAsync(userId, targetId.ToGuid(), filter);
            return res.ToActionResult();
        }
    }
}
