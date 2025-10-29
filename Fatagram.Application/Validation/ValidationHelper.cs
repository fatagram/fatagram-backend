using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Application.Exceptions;
using Fatagram.Application.Exceptions.DetailExceptions;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace Fatagram.Application.Validation
{
    public static class ValidationHelper
    {
        public static void EnsureValidUrlName(string urlName)
        {
            if (string.IsNullOrWhiteSpace(urlName))
                throw new ValidateException("URL_NAME_EMPTY", null, "UrlName cannot be empty.");

            if (urlName.Length < 3)
                throw new ValidateException(
                    "URL_NAME_TOO_SHORT",
                    null,
                    "UrlName must be between 3 and 36 characters."
                );

            if (urlName.Length > 36)
                throw new ValidateException(
                    "URL_NAME_TOO_LONG",
                    null,
                    "UrlName must be between 3 and 36 characters."
                );

            if (urlName.Contains(" "))
                throw new ValidateException(
                    "URL_NAME_CONTAINS_SPACE",
                    null,
                    "UrlName cannot contain spaces."
                );
        }

        public static void EnsureValidNickname(string nickname)
        {
            if (nickname.Length > 20)
                throw new ValidateException(
                    "NICKNAME_TOO_LONG",
                    null,
                    "Nickname must be between 3 and 30 characters."
                );
        }
    }
}
