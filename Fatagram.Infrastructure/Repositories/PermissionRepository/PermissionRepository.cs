using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Data;
using Fatagram.Infrastructure.Repositories.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Fatagram.Infrastructure.Repositories.PermissionRepository
{
    public class PermissionRepository(AppDbContext dbContext, ILogger<PermissionRepository> logger)
        : BaseRepository<Permission>(dbContext, logger),
            IPermissionRepository
    {
        public async Task<IEnumerable<string>> GetPermissionNamesAsync(
            Guid userId,
            Guid? resourceId,
            CancellationToken ct = default
        )
        {
            var query = _dbContext
                .UserRoles.Where(ur => ur.UserId == userId && ur.ResourceId == resourceId)
                .SelectMany(ur => ur.Role.Permissions.Select(p => p.Name));

            return await query.Distinct().ToListAsync(ct);
        }
    }
}
