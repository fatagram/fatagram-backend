using System.Security.Claims;
using Fatagram.Application.Utils;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Fatagram.Application.Services.JwtServices.Interface
{
    /// <summary>
    /// Interface for the JWT service
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="jwtSettings"></param>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Result<string> GenerateToken(string username, Guid userId);

        /// <summary>
        /// Validate a JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Result<string> GenerateToken(string username, string userId);

        /// <summary>
        /// Validate a JWT token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Result<ClaimsPrincipal> ValidateToken(string token);
    }
}
