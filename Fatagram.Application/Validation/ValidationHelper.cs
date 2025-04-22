using Fatagram.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Validation
{
    public static class ValidationHelper
    {
        public static void EnsureValidUrlName(string urlName)
        {
            if (string.IsNullOrWhiteSpace(urlName))
                throw new ValidateException("URL_NAME_EMPTY", "UrlName cannot be empty.");

            if (urlName.Length < 3)
                throw new ValidateException("URL_NAME_TOO_SHORT", "UrlName must be between 3 and 36 characters.");

            if (urlName.Length > 36)
                throw new ValidateException("URL_NAME_TOO_LONG", "UrlName must be between 3 and 36 characters.");

            if (urlName.Contains(" "))
                throw new ValidateException("URL_NAME_CONTAINS_SPACE", "UrlName cannot contain spaces.");
        }
    }
}
