using System.Security.Claims;
using Fatagram.API.Utils.Attributes;
using Fatagram.API.Utils.Response;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace Fatagram.API.Middlewares
{
    public class CheckOnboardingMiddleware(
        RequestDelegate next,
        ILogger<CheckOnboardingMiddleware> logger
    )
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<CheckOnboardingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            var metadata = endpoint?.Metadata;
            if (
                metadata?.GetMetadata<NotRequireOnBoardingAttribute>() != null
                || metadata?.GetMetadata<AuthorizeAttribute>() == null
                || context.Request.Path.Value?.Contains("/admin") == true
            )
            {
                await _next(context);
                return;
            }
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier).ToGuid();
                _logger.LogInformation("[CheckOnboardingMiddleware] User ID: {UserId}", userId);
                if (userId != Guid.Empty)
                {
                    var userProfileService =
                        context.RequestServices.GetRequiredService<IUserProfileService>();
                    var isOnboardingCompleted = await userProfileService.IsOnboardingCompletedAsync(
                        userId
                    );
                    _logger.LogInformation(
                        "[CheckOnboardingMiddleware] Is onboarding completed: {IsOnboardingCompleted}",
                        isOnboardingCompleted.Data
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
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsync(errorResponse.ToString());
        }
    }
}
