using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.MiddleLevelExceptions
{
    public class ForbiddenException(Error error) : AppException(error) { }
}
