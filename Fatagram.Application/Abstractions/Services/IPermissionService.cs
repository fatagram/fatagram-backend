using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Services
{
    public interface IPermissionService
    {
        /// <summary>Returns true if the user has the required permission, checking both global and resource-scoped roles.</summary>
        Task<bool> HasPermissionAsync(
            Guid userId,
            string permissionName,
            Guid? resourceId,
            CancellationToken ct = default
        );

        /// <summary>Returns active route permission rules, cached for 5 minutes.</summary>
        Task<IEnumerable<RoutePermission>> GetActiveRoutePermissionsAsync(
            CancellationToken ct = default
        );

        /// <summary>Invalidates all cached permission data for a user.</summary>
        Task InvalidateUserPermissionCacheAsync(Guid userId);
    }
}
