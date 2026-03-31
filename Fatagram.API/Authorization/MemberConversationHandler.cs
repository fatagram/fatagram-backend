using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Cache;
using Fatagram.Infrastructure.Repositories.ConversationParticipantRepository.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Fatagram.API.Authorization
{
    public class MemberConversationHandler : AuthorizationHandler
    {
        public override async Task<bool> HandleAsync(HttpContext context, ResourceAuth metadata)
        {
            var userIdStr = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
                return false;

            var routeParams = ExtractRouteParameters(context, metadata.RouteKey);
            if (!routeParams.TryGetValue(metadata.RouteKey, out var resourceId))
                return false;

            if (!Guid.TryParse(resourceId, out var conversationId))
                return false;

            if (!Guid.TryParse(userIdStr, out var userId))
                return false;

            // Try cache first
            var cache = context.RequestServices.GetService<ICacheService>();
            if (cache != null)
            {
                var cacheKey = $"conversation:{conversationId}:participants";
                var cachedParticipants = await cache.GetAsync<List<ParticipantDto>>(cacheKey);
                if (cachedParticipants != null)
                {
                    return cachedParticipants.Any(p => p.UserId == userId);
                }
            }

            // Fallback to repository DB check
            var repo = context.RequestServices.GetService<IConversationParticipantRepository>();
            if (repo == null)
                return false;

            var count = await repo.CountAsync(cp =>
                cp.ConversationId == conversationId && cp.UserId == userId
            );
            return count > 0;
        }
    }
}
