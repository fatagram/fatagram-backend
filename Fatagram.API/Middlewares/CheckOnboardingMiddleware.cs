using Fatagram.API.Utils.Attributes;
using Fatagram.API.Utils.Response;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Middlewares
{
    public class CheckOnboardingMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

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
            var errorResponse = ErrorResponse.Create(
                new ErrorDetails()
                {
                    Code = "ONBOARDING_NOT_COMPLETED",
                    Detail = "Onboarding not completed.",
                }
            );
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(errorResponse.ToString());
        }
    }
}
