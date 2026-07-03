using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Services.UserServices.UserServices;
using Fatagram.Application.Utils;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.UserServices.UserServices
{
    public class UserService(
        IUserRepository userRepository,
        IMapper mapper,
        ILogger<UserService> logger
    ) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<UserService> _logger = logger;

        public async Task<Result<CursorResult<UserDto, DateTime>>> GetAllAsync(
            CursorFilter<DateTime> filter
        )
        {
            var users = await _userRepository.GetAllAsync(
                u => new UserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Avatar = u.Avatar,
                    CreatedAt = u.CreatedAt,
                },
                u => u.FullName!.Contains(filter.Keyword ?? string.Empty),
                u => u.CreatedAt,
                filter.SortDesc ?? true,
                filter.Limit,
                filter.Cursor
            );
            return Result<CursorResult<UserDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<UserDto, DateTime>
                {
                    Items = users,
                    NextCursor = users.Count > 0 ? users.Last().CreatedAt : null,
                    HasNext = users.Count == filter.Limit,
                }
            );
        }

        public async Task<Result<CursorResult<SearchUserDto, DateTime>>> SearchAsync(
            Guid currentUserId,
            CursorFilter<DateTime> filter
        )
        {
            var users = await _userRepository.GetAllAsync(
                u => new SearchUserDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    UrlName = u.UrlName,
                    Avatar = u.Avatar,
                    CreatedAt = u.CreatedAt,
                    Status =
                        (
                            u.FriendshipAsUser1.Any(f => f.User2Id == currentUserId)
                            || u.FriendshipAsUser2.Any(f => f.User1Id == currentUserId)
                        )
                            ? FriendShipStatus.Friend
                        : u.FriendRequestsReceived.Any(fr => fr.SenderId == currentUserId)
                            ? FriendShipStatus.SentByMe
                        : u.FriendRequests.Any(fr => fr.ReceiverId == currentUserId)
                            ? FriendShipStatus.SentByThem
                        : FriendShipStatus.None,
                },
                u =>
                    u.Id != currentUserId
                    && (
                        string.IsNullOrEmpty(filter.Keyword)
                        || u.FullName!.Contains(filter.Keyword)
                        || u.UrlName == filter.Keyword
                    ),
                u => u.CreatedAt,
                filter.SortDesc ?? true,
                filter.Limit,
                filter.Cursor
            );

            return Result<CursorResult<SearchUserDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<SearchUserDto, DateTime>
                {
                    Items = users,
                    NextCursor = users.Count > 0 ? users.Last().CreatedAt : null,
                    HasNext = users.Count == filter.Limit,
                }
            );
        }
    }
}
