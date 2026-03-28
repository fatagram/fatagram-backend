using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Services.UserServices.UserServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.UserRepository.Interface;
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
    }
}
