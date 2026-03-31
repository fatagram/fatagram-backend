using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Fatagram.API.Authorization;
using Fatagram.API.Utils;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace Fatagram.API.Middlewares
{
    public class AuthorizationMiddleware(
        RequestDelegate next,
        ILogger<AuthorizationMiddleware> logger
    )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<AuthorizationMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null)
            {
                await _next(context);
                return;
            }

            var authMetadata = endpoint.Metadata.GetMetadata<ResourceAuth>();
            Console.WriteLine(
                $"AuthorizationMiddleware - Endpoint: {endpoint.DisplayName}, ResourceAuth: {authMetadata?.ResourceType}, RouteKey: {authMetadata?.RouteKey}"
            );
            if (authMetadata == null)
            {
                await _next(context);
                return;
            }

            var authHandler = GetAuthorizationHandlerForEndpoint(authMetadata);
            if (authHandler == null)
            {
                throw new ForbiddenException(
                    new Error(
                        "AUTHORIZATION_HANDLER_NOT_FOUND",
                        "No authorization handler found for this endpoint"
                    )
                );
            }

            var isAuthorized = await authHandler.HandleAsync(context, authMetadata);
            if (!isAuthorized)
            {
                throw new ForbiddenException(
                    new Error("FORBIDDEN", "You don't have permission to access this resource.")
                );
            }

            // If authorized, continue to the next middleware
            await _next(context);
        }

        private AuthorizationHandler? GetAuthorizationHandlerForEndpoint(ResourceAuth authMetadata)
        {
            switch (authMetadata?.ResourceType)
            {
                case "Owner":
                    return new OwnerAuthorizationHandler();
                case "MemberConversation":
                    return new MemberConversationHandler();
                default:
                    return null;
            }
        }
    }
}
