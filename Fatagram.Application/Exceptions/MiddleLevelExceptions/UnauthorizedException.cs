using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.MiddleLevelExceptions
{
    public class UnauthorizedException()
        : AppException(new Error("UNAUTHORIZED", "Unauthorized")) { }
}
