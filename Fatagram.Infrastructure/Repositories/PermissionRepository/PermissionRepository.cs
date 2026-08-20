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
            var permissions = new HashSet<string>();

            // 1. Get permissions from explicit user_roles (Global or Scoped)
            var explicitPermissions = await _dbContext
                .UserRoles.Where(ur => ur.UserId == userId && (ur.ResourceId == resourceId || ur.ResourceId == null))
                .SelectMany(ur => ur.Role.Permissions.Select(p => p.Name))
                .ToListAsync(ct);

            foreach (var p in explicitPermissions)
                permissions.Add(p);

            // 2. If resourceId is a conversation, resolve the role configured dynamically in database
            if (resourceId.HasValue)
            {
                var participant = await _dbContext.ConversationParticipants
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cp => cp.ConversationId == resourceId.Value && cp.UserId == userId, ct);

                if (participant != null)
                {
                    var roleName = participant.Role switch
                    {
                        Fatagram.Domain.Enums.ConversationRole.Owner => "Conversation Owner",
                        Fatagram.Domain.Enums.ConversationRole.Admin => "Conversation Admin",
                        _ => "Conversation Member"
                    };

                    // Query the dynamic permissions configured in Web Admin for this role!
                    var rolePerms = await _dbContext.Roles
                        .Where(r => r.Name == roleName)
                        .SelectMany(r => r.Permissions.Select(p => p.Name))
                        .ToListAsync(ct);

                    // Fallback: If roles haven't been seeded yet, default to standard member permissions
                    if (rolePerms.Count == 0)
                    {
                        permissions.Add("conversation.member");
                        permissions.Add("conversation.send_message");
                    }
                    else
                    {
                        foreach (var p in rolePerms)
                            permissions.Add(p);
                    }
                }

                // Check user owner
                if (resourceId.Value == userId)
                {
                    permissions.Add("user.owner");
                }
            }

            return permissions;
        }
    }
}
