using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Shared.Common;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class RefreshTokenExpiredException()
        : BadRequestException(new Error("REFRESH_TOKEN_EXPIRED", "Refresh token is expired")) { }
}
