using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class DataNullException()
        : BadRequestException(new Error("DATA_NULL", "Data is null")) { }
}
