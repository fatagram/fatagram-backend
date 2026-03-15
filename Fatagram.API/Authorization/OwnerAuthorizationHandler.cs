using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Fatagram.API.Utils;

namespace Fatagram.API.Authorization
{
    public class OwnerAuthorizationHandler : AuthorizationHandler
    {
        public override async Task<bool> HandleAsync(HttpContext context, ResourceAuth metadata)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            var routeParams = ExtractRouteParameters(context, metadata.RouteKey);
            if (!routeParams.TryGetValue(metadata.RouteKey, out var resourceId))
            {
                return false;
            }
            Console.WriteLine(
                $"OwnerAuthorizationHandler - UserId: {userId}, ResourceId: {resourceId}, RouteKey: {metadata.RouteKey}"
            );

            // Compare the userId in param with the userId from the token
            return string.Equals(userId, resourceId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
