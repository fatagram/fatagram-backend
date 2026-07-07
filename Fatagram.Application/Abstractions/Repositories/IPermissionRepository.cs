using Fatagram.Domain.Models;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IPermissionRepository : IBaseRepository<Permission>
    {
        /// <summary>Returns all permission names granted to a user, optionally scoped to a resource.</summary>
        Task<IEnumerable<string>> GetPermissionNamesAsync(
            Guid userId,
            Guid? resourceId,
            CancellationToken ct = default
        );
    }
}
