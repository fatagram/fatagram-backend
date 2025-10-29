using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Exceptions.DetailExceptions
{
    public class RefreshTokenExpiredException : AppException
    {
        public RefreshTokenExpiredException()
            : base("REFRESH_TOKEN_EXPIRED", "Refresh token is expired") { }
    }
}
