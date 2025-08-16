using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.UserServices.FriendshipServices.Interface;
using Fatagram.Application.Services.UserServices.UserProfileServices.Interface;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Controllers.UserControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendshipController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        public FriendshipController(
            IFriendshipService friendshipService
        )
        {
            _friendshipService = friendshipService;
        }

        [Authorize]
        [HttpPost("add/{receiverId}")]
        public async Task<IActionResult> AddFriend([FromRoute] string receiverId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _friendshipService.SendAddFriendAsync(userId.ToGuid(), receiverId.ToGuid());
            return result.ToActionResult();
        }

        [Authorize]
        [HttpPost("accept/{senderId}")]
        public async Task<IActionResult> AcceptFriend([FromRoute] string senderId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _friendshipService.AcceptAddFriendAsync(userId.ToGuid(), senderId.ToGuid());
            return result.ToActionResult();
        }

        [Authorize]
        [HttpGet("status/{targetId}")]
        public async Task<IActionResult> GetFriendshipStatus([FromRoute] string targetId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _friendshipService.GetFriendshipStatusAsync(userId.ToGuid(), targetId.ToGuid());
            return res.ToActionResult();
        }

        [Authorize]
        [HttpDelete("cancel/{senderId}")]
        public async Task<IActionResult> CancelAddFriendRequest([FromRoute] string senderId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _friendshipService.CancelAddFriendAsync(userId.ToGuid(), senderId.ToGuid());
            return res.ToActionResult();
        }

        [Authorize]
        [HttpDelete("decline/{requesterId}")]
        public async Task<IActionResult> DeclineAddFriendRequest([FromRoute] string requesterId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _friendshipService.DeclineAddFriendRequestAsync(userId.ToGuid(), requesterId.ToGuid());
            return res.ToActionResult();
        }

        [Authorize]
        [HttpDelete("unfriend/{friendId}")]
        public async Task<IActionResult> Unfriend([FromRoute] string friendId)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _friendshipService.UnfriendAsync(userId.ToGuid(), friendId.ToGuid());
            return res.ToActionResult();
        }

        [HttpGet("count/{targetId}")]
        public async Task<IActionResult> GetNumberOfFriends(string targetId)
        {
            if (HttpContext.User.Identity?.IsAuthenticated ?? false)
            {
                // var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                // if (userId == null) userId = Guid.Empty.ToString();
                var _res = await _friendshipService.GetNumberOfFriendsAsync(targetId.ToGuid());
                return _res.ToActionResult();
            }
            var res = await _friendshipService.GetNumberOfFriendsAsync(targetId.ToGuid());
            return res.ToActionResult();
        }

        [Authorize]
        [HttpGet("requests")]
        public async Task<IActionResult> GetFriendRequests([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new UnauthorizedException();
            }
            var res = await _friendshipService.GetFriendRequestsAsync(userId.ToGuid(), page, pageSize);
            return res.ToActionResult();
        }


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