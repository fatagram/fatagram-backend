using System.Security.Claims;

namespace Fatagram.Application.Abstractions.Security
{
    public interface IJwtTokenValidator
    {
        ClaimsPrincipal? ValidateToken(string token);
    }
}
