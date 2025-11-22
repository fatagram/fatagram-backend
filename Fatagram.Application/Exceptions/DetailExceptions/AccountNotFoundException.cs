using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class AccountNotFoundException()
        : NotFoundException(new Error("ACCOUNT_NOT_FOUND", "Account not found")) { }
}
