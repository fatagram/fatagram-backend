using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Controllers.V1;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.V1.UserControllers
{
    [Route("api/[controller]")]
    public class FriendshipController : BaseApiController
    {
        private readonly IFriendshipService _friendshipService;
        public FriendshipController(
            IFriendshipService friendshipService
        )
        {
            _friendshipService = friendshipService;
        }

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
            var result = await _friendshipService.SendAddFriendAsync(userId, receiverId.ToGuid());
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
            var result = await _friendshipService.AcceptAddFriendAsync(userId, senderId.ToGuid());
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
            var res = await _friendshipService.CancelAddFriendAsync(userId, senderId.ToGuid());
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
            var res = await _friendshipService.DeclineAddFriendRequestAsync(userId, requesterId.ToGuid());
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
            ValidateModelState();
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
        public async Task<IActionResult> GetFriendRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetCurrentUserId();
            var res = await _friendshipService.GetFriendRequestsAsync(userId, page, pageSize);
            return res.ToActionResult();
        }

        /// <summary>
        /// Get a list of friends for a specific user, with optional keyword search and pagination.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("friends/{userId}")]
        public async Task<IActionResult> GetFriends(string userId, string keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                var _userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (_userId == null) _userId = Guid.Empty.ToString();
                var res = await _friendshipService.GetFriendsAsync(_userId.ToGuid(), userId.ToGuid(), keyword, page, pageSize);
                return res.ToActionResult();
            }
            else
            {
                var res = await _friendshipService.GetFriendsAsync(Guid.Empty, userId.ToGuid(), keyword, page, pageSize);
                return res.ToActionResult();
            }
        }
    }
}