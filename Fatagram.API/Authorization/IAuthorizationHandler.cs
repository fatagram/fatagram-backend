using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Utils;
using Microsoft.Extensions.ObjectPool;

namespace Fatagram.API.Authorization
{
    public abstract class AuthorizationHandler
    {
        public abstract Task<bool> HandleAsync(HttpContext context, ResourceAuth metadata);

        // Extract route parameters based on the RouteKey in metadata
        protected Dictionary<string, string> ExtractRouteParameters(
            HttpContext context,
            string routeKey
        )
        {
            var routeValues = context.Request.RouteValues;
            var parameters = new Dictionary<string, string>();
            if (routeValues.TryGetValue(routeKey, out var value))
            {
                parameters[routeKey] = value?.ToString() ?? string.Empty;
            }
            return parameters;
        }
    }
}
