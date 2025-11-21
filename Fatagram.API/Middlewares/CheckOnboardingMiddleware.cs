using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.API.Attributes;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;

namespace Fatagram.API.Middlewares
{
    public class CheckOnboardingMiddleware
    {
        private readonly RequestDelegate _

        public CheckOnboardingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var metadata = endpoint?.Metadata;
            if (
                metadata?.GetMetadata<NotRequireOnBoardingAttribute>() != null
                || metadata?.GetMetadata<AuthorizeAttribute>() == null
            )
            {
                await _next(context);
                return;
            }
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst("sub")?.Value;
                if (userId != null)
                {
                    var userProfileService =
                        context.RequestServices.GetRequiredService<IUserProfileService>();
                    var isOnboardingCompleted = await userProfileService.IsOnboardingCompletedAsync(
                        userId.ToGuid()
                    );
                    if (isOnboardingCompleted.Data)
                    {
                        await _next(context);
                        return;
                    }
                }
            }
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync("Onboarding not completed.");
            return;
        }
    }
}
