using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.JwtServices
{
    public interface IJwtService
    {
        Result<string> GenerateToken(Guid userId);
    }
}
