using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.MiddleLevelExceptions
{
    public class BadRequestException(Error error, List<Error>? errors = null)
        : AppException(error, errors) { }
}
