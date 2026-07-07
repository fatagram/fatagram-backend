using Fatagram.Application.Abstractions.Cache;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Application.Abstractions.Services;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Services.PermissionServices
{
    public class PermissionService(
        IPermissionRepository permissionRepository,
        IRoutePermissionRepository routePermissionRepository,
        ICacheService cacheService
    ) : IPermissionService
    {
        private const int PermCacheTtlSeconds = 60;
        private const int RouteCacheTtlSeconds = 300;

        public async Task<bool> HasPermissionAsync(
            Guid userId,
            string permissionName,
            Guid? resourceId,
            CancellationToken ct = default
        )
        {
            var cacheKey = BuildPermCacheKey(userId, permissionName, resourceId);
            if (await cacheService.ExistsAsync(cacheKey))
                return await cacheService.GetAsync<bool>(cacheKey);

            var globalPermissions = await permissionRepository.GetPermissionNamesAsync(
                userId,
                null,
                ct
            );
            if (globalPermissions.Contains(permissionName))
            {
                await cacheService.SetAsync(
                    cacheKey,
                    true,
                    TimeSpan.FromSeconds(PermCacheTtlSeconds)
                );
                return true;
            }

            if (resourceId.HasValue)
            {
                var scopedPermissions = await permissionRepository.GetPermissionNamesAsync(
                    userId,
                    resourceId,
                    ct
                );
                if (scopedPermissions.Contains(permissionName))
                {
                    await cacheService.SetAsync(
                        cacheKey,
                        true,
                        TimeSpan.FromSeconds(PermCacheTtlSeconds)
                    );
                    return true;
                }
            }

            await cacheService.SetAsync(cacheKey, false, TimeSpan.FromSeconds(PermCacheTtlSeconds));
            return false;
        }

        public async Task InvalidateUserPermissionCacheAsync(Guid userId)
        {
            var keys = await cacheService.GetKeysAsync($"perm:{userId}:*");
            foreach (var key in keys)
                await cacheService.RemoveAsync(key);
        }

        public async Task<IEnumerable<RoutePermission>> GetActiveRoutePermissionsAsync(
            CancellationToken ct = default
        )
        {
            const string cacheKey = "route_permissions:active";
            var cached = await cacheService.GetAsync<List<RoutePermission>>(cacheKey);
            if (cached is not null)
                return cached;

            var permissions = (
                await routePermissionRepository.GetActivePermissionsAsync(ct)
            ).ToList();
            await cacheService.SetAsync(
                cacheKey,
                permissions,
                TimeSpan.FromSeconds(RouteCacheTtlSeconds)
            );
            return permissions;
        }

        private static string BuildPermCacheKey(Guid userId, string permName, Guid? resourceId) =>
            $"perm:{userId}:{permName}:{(resourceId.HasValue ? resourceId.Value.ToString() : "global")}";
    }
}
