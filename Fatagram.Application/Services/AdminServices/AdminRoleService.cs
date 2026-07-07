using Fatagram.Application.Abstractions.Cache;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Dtos.Admin;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Shared.Common;
using Fatagram.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Application.Services.AdminServices
{
    public class AdminRoleService(
        IBaseRepository<Role> roleRepository,
        IBaseRepository<Permission> permissionRepository,
        ICacheService cacheService
    ) : IAdminRoleService
    {
        public async Task<Result<List<RoleDto>>> GetAllRolesAsync(CancellationToken ct = default)
        {
            var roles = await roleRepository.GetAllAsync<Role>(
                include: q => q.Include(r => r.Permissions));
            return Result<List<RoleDto>>.Create(data: roles.Select(RoleDto.From).ToList());
        }

        public async Task<Result<List<PermissionDto>>> GetAllPermissionsAsync(CancellationToken ct = default)
        {
            var perms = await permissionRepository.GetAllAsync<Permission>();
            return Result<List<PermissionDto>>.Create(data: perms.Select(PermissionDto.From).ToList());
        }

        public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto dto, CancellationToken ct = default)
        {
            var role = new Role { Name = dto.Name, Description = dto.Description };
            var created = await roleRepository.AddAsync(role);
            return Result<RoleDto>.Create(ResponseStatusCode.Created, RoleDto.From(created));
        }

        public async Task<Result> DeleteRoleAsync(Guid roleId, CancellationToken ct = default)
        {
            var role = await roleRepository.GetByUniqueAsync<Role>(r => r.Id == roleId);
            if (role is null)
                throw new NotFoundException(new Error("ROLE_NOT_FOUND", "Role not found"));
            if (role.IsSystem)
                throw new BadRequestException(new Error("SYSTEM_ROLE", "Cannot delete a system role"));

            await roleRepository.DeleteAsync(roleId);
            return Result.Create();
        }

        public async Task<Result> AssignPermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
        {
            var role = await roleRepository.GetByUniqueAsync<Role>(
                r => r.Id == roleId,
                selector: null,
                include: q => q.Include(r => r.Permissions));
            if (role is null)
                throw new NotFoundException(new Error("ROLE_NOT_FOUND", "Role not found"));

            var permission = await permissionRepository.GetByUniqueAsync<Permission>(p => p.Id == permissionId);
            if (permission is null)
                throw new NotFoundException(new Error("PERMISSION_NOT_FOUND", "Permission not found"));

            if (!role.Permissions.Any(p => p.Id == permissionId))
            {
                role.Permissions.Add(permission);
                await roleRepository.UpdateAsync(role);
                await InvalidateRolePermissionCacheAsync(roleId);
            }

            return Result.Create();
        }

        public async Task<Result> RemovePermissionAsync(Guid roleId, Guid permissionId, CancellationToken ct = default)
        {
            var role = await roleRepository.GetByUniqueAsync<Role>(
                r => r.Id == roleId,
                selector: null,
                include: q => q.Include(r => r.Permissions));
            if (role is null)
                throw new NotFoundException(new Error("ROLE_NOT_FOUND", "Role not found"));

            var perm = role.Permissions.FirstOrDefault(p => p.Id == permissionId);
            if (perm is not null)
            {
                role.Permissions.Remove(perm);
                await roleRepository.UpdateAsync(role);
                await InvalidateRolePermissionCacheAsync(roleId);
            }

            return Result.Create();
        }

        private async Task InvalidateRolePermissionCacheAsync(Guid roleId)
        {
            // Invalidate all permission cache entries for users who have this role
            var keys = await cacheService.GetKeysAsync("perm:*");
            foreach (var key in keys)
                await cacheService.RemoveAsync(key);
        }
    }
}
