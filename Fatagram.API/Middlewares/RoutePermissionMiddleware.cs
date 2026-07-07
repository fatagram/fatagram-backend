using System.Security.Claims;
using Fatagram.Application.Abstractions.Services;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Domain.Models;
using Fatagram.Shared.Common;

namespace Fatagram.API.Middlewares
{
    public class RoutePermissionMiddleware(RequestDelegate next)
    {
        public async Task InvokeAsync(HttpContext context, IPermissionService permissionService)
        {
            var rules = await permissionService.GetActiveRoutePermissionsAsync(
                context.RequestAborted
            );
            var path = context.Request.Path.Value?.TrimStart('/') ?? string.Empty;
            var method = context.Request.Method;

            var (matchedRule, routeValues) = FindMatch(rules, path, method);
            if (matchedRule is null)
            {
                await next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedException();

            var userIdStr = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                throw new UnauthorizedException();

            Guid? resourceId = null;
            if (
                matchedRule.ResourceParam is not null
                && routeValues.TryGetValue(matchedRule.ResourceParam, out var resourceIdStr)
                && Guid.TryParse(resourceIdStr, out var rid)
            )
            {
                resourceId = rid;
            }

            var hasPermission = await permissionService.HasPermissionAsync(
                userId,
                matchedRule.PermissionName,
                resourceId,
                context.RequestAborted
            );

            if (!hasPermission)
                throw new ForbiddenException(
                    new Error("FORBIDDEN", "You don't have permission to access this resource.")
                );

            await next(context);
        }

        private static (RoutePermission? Rule, Dictionary<string, string> RouteValues) FindMatch(
            IEnumerable<RoutePermission> rules,
            string path,
            string method
        )
        {
            foreach (var rule in rules)
            {
                if (
                    rule.HttpMethod != "*"
                    && !string.Equals(rule.HttpMethod, method, StringComparison.OrdinalIgnoreCase)
                )
                    continue;

                var routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (MatchPattern(rule.RoutePattern, path, routeValues))
                    return (rule, routeValues);
            }
            return (null, []);
        }

        private static bool MatchPattern(
            string pattern,
            string path,
            Dictionary<string, string> captured
        )
        {
            var patternParts = pattern.Split('/');
            var pathParts = path.Split('/');

            for (var i = 0; i < patternParts.Length; i++)
            {
                var pp = patternParts[i];

                if (pp == "*")
                    return true;

                if (i >= pathParts.Length)
                    return false;

                var seg = pathParts[i];

                if (pp.StartsWith('{') && pp.EndsWith('}'))
                    captured[pp[1..^1]] = seg;
                else if (!string.Equals(pp, seg, StringComparison.OrdinalIgnoreCase))
                    return false;
            }

            return patternParts.Length == pathParts.Length;
        }
    }
}
