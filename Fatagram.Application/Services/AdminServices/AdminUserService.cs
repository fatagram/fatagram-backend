using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Abstractions.Services;
using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Application.Services.AdminServices
{
    public class AdminUserService(
        IBaseRepository<User> userRepository,
        IBaseRepository<UserRole> userRoleRepository,
        IBaseRepository<Role> roleRepository,
        IPermissionService permissionService
    ) : IAdminUserService
    {
        public async Task<Result<List<UserAdminDto>>> SearchUsersAsync(string query, CancellationToken ct = default)
        {
            var users = await userRepository.GetAllAsync<User>(
                filter: u => string.IsNullOrEmpty(query)
                    || (u.FullName != null && u.FullName.Contains(query))
                    || (u.UrlName != null && u.UrlName.Contains(query)),
                include: q => q.Include(u => u.Accounts),
                selector: null);

            var userIds = users.Select(u => u.Id).ToList();
            var userRoles = await userRoleRepository.GetAllAsync<UserRole>(
                filter: ur => userIds.Contains(ur.UserId) && ur.ResourceId == null,
                include: q => q.Include(ur => ur.Role));

            var rolesByUser = userRoles
                .GroupBy(ur => ur.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var dtos = users.Select(u =>
            {
                var roles = rolesByUser.TryGetValue(u.Id, out var r)
                    ? r.Select(ur => new UserRoleDto(ur.RoleId, ur.Role.Name, ur.ResourceId)).ToList()
                    : new List<UserRoleDto>();
                return new UserAdminDto(
                    u.Id,
                    u.UrlName,
                    u.FullName,
                    u.Accounts.FirstOrDefault()?.Username,
                    roles);
            }).ToList();

            return Result<List<UserAdminDto>>.Create(data: dtos);
        }

        public async Task<Result<List<RoleDto>>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
        {
            var userRoles = await userRoleRepository.GetAllAsync<UserRole>(
                filter: ur => ur.UserId == userId,
                include: q => q.Include(ur => ur.Role).ThenInclude(r => r.Permissions));

            var roles = userRoles.Select(ur => RoleDto.From(ur.Role)).ToList();
            return Result<List<RoleDto>>.Create(data: roles);
        }

        public async Task<Result> AssignRoleAsync(Guid userId, AssignRoleDto dto, CancellationToken ct = default)
        {
            var roleExists = await roleRepository.GetByUniqueAsync<Role>(r => r.Id == dto.RoleId);
            if (roleExists is null)
                throw new NotFoundException(new Error("ROLE_NOT_FOUND", "Role not found"));

            var existing = await userRoleRepository.GetByUniqueAsync<UserRole>(
                ur => ur.UserId == userId && ur.RoleId == dto.RoleId && ur.ResourceId == dto.ResourceId);
            if (existing is not null)
                return Result.Create();

            await userRoleRepository.AddAsync(new UserRole
            {
                UserId = userId,
                RoleId = dto.RoleId,
                ResourceId = dto.ResourceId,
            });

            await permissionService.InvalidateUserPermissionCacheAsync(userId);
            return Result.Create();
        }

        public async Task<Result> RemoveRoleAsync(Guid userId, Guid roleId, Guid? resourceId, CancellationToken ct = default)
        {
            var userRole = await userRoleRepository.GetByUniqueAsync<UserRole>(
                ur => ur.UserId == userId && ur.RoleId == roleId && ur.ResourceId == resourceId);
            if (userRole is null)
                return Result.Create();

            await userRoleRepository.DeleteAsync(userRole);
            await permissionService.InvalidateUserPermissionCacheAsync(userId);
            return Result.Create();
        }
    }
}
