using System.Security.Claims;
using Fatagram.API.Extensions.Constrains;
using Fatagram.Application.Exceptions.DetailExceptions;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Fatagram.API.Controllers.V1
{
    /// <summary>
    /// Base controller providing common functionality for all API controllers
    /// </summary>
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        protected bool _isSecureCookies = !Setups.IsForLAN;

        /// <summary>
        /// Gets the current user ID from JWT claims. Throws UnauthorizedException if not found.
        /// Use this when user authentication is required.
        /// </summary>
        /// <returns>Current user's GUID</returns>
        /// <exception cref="UnauthorizedException">Thrown when user is not authenticated</exception>
        protected Guid GetCurrentUserId()
        {
            var userId = GetCurrentUserIdOrNull();
            if (userId == null)
            {
                throw new UnauthorizedException("User is not authenticated");
            }
            return userId.Value;
        }

        /// <summary>
        /// Gets the current user ID from JWT claims. Returns null if not found.
        /// Use this when user authentication is optional.
        /// </summary>
        /// <returns>Current user's GUID or null if not authenticated</returns>
        protected Guid? GetCurrentUserIdOrNull()
        {
            var userIdClaim = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return userIdClaim.ToGuid() == Guid.Empty ? null : userIdClaim.ToGuid();
        }

        /// <summary>
        /// Gets the current user ID as string from JWT claims. Returns null if not found.
        /// Use this when you need the raw string value.
        /// </summary>
        /// <returns>Current user's ID as string or null if not authenticated</returns>
        protected string? GetCurrentUserIdAsString()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        /// <summary>
        /// Checks if the current user is authenticated
        /// </summary>
        /// <returns>True if user is authenticated, false otherwise</returns>
        protected bool IsUserAuthenticated()
        {
            return !string.IsNullOrEmpty(
                HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
            );
        }

        /// <summary>
        /// Gets the current user's username from JWT claims
        /// </summary>
        /// <returns>Username or null if not found</returns>
        protected string? GetCurrentUsername()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.Name);
        }

        /// <summary>
        /// Gets the current user's email from JWT claims
        /// </summary>
        /// <returns>Email or null if not found</returns>
        protected string? GetCurrentUserEmail()
        {
            return HttpContext.User.FindFirstValue(ClaimTypes.Email);
        }

        /// <summary>
        /// Gets a specific claim value from the current user's JWT token
        /// </summary>
        /// <param name="claimType">The type of claim to retrieve</param>
        /// <returns>Claim value or null if not found</returns>
        protected string? GetClaimValue(string claimType)
        {
            return HttpContext.User.FindFirstValue(claimType);
        }

        /// <summary>
        /// Checks if the current user has a specific role
        /// </summary>
        /// <param name="role">Role to check</param>
        /// <returns>True if user has the role, false otherwise</returns>
        protected bool HasRole(string role)
        {
            return HttpContext.User.IsInRole(role);
        }

        /// <summary>
        /// Checks if the current user has any of the specified roles
        /// </summary>
        /// <param name="roles">Roles to check</param>
        /// <returns>True if user has any of the roles, false otherwise</returns>
        protected bool HasAnyRole(params string[] roles)
        {
            return roles.Any(role => HttpContext.User.IsInRole(role));
        }

        /// <summary>
        /// Checks if the current user has all of the specified roles
        /// </summary>
        /// <param name="roles">Roles to check</param>
        /// <returns>True if user has all roles, false otherwise</returns>
        protected bool HasAllRoles(params string[] roles)
        {
            return roles.All(role => HttpContext.User.IsInRole(role));
        }

        #region Model Validation Methods

        /// <summary>
        /// Validates ModelState and throws ValidateException if invalid.
        /// Use this when you want to throw exception for invalid models.
        /// </summary>
        /// <exception cref="ValidateException">Thrown when ModelState is invalid</exception>
        protected void ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = GetModelStateErrors();
                throw new ValidateException("VALIDATION_FAILED", errors, "Model validation failed");
            }
        }

        /// <summary>
        /// Gets all ModelState errors as a list of strings
        /// </summary>
        /// <returns>List of error messages</returns>
        protected List<string> GetModelStateErrors()
        {
            return ModelState
                .Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .Where(msg => !string.IsNullOrEmpty(msg))
                .ToList();
        }

        #endregion
    }
}
