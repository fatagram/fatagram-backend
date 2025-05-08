using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Utils
{
    /// <summary>
    /// Provides regular expression patterns for validating various fields.
    /// </summary>
    public static class RegexPatterrns
    {
        /// <summary>
        /// Regular expression pattern for validating usernames.
        /// Allows alphanumeric characters and underscores, with a length between 3 and 20 characters.
        /// </summary>
        public const string Username = @"^[a-zA-Z0-9_]{3,20}$";

        /// <summary>
        /// Regular expression pattern for validating passwords.
        /// Requires at least 8 non-whitespace characters.
        /// </summary>
        public const string Password = @"^\S{8,}$";

        /// <summary>
        /// Regular expression pattern for validating email addresses.
        /// </summary>
        public const string Email = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

        /// <summary>
        /// Regular expression pattern for validating phone numbers.
        /// Requires at least 10 digits.
        /// </summary>
        public const string Phone = @"^0[0-9]{9,}$";

        /// <summary>
        /// Regular expression pattern for validating last names.
        /// Allows only alphabetic characters, including Vietnamese characters.
        /// </summary>
        public const string LastName = @"^[\p{L}]+$";

        /// <summary>
        /// Regular expression pattern for validating first names.
        /// Allows only alphabetic characters, including Vietnamese characters.
        /// </summary>
        public const string FirstName = @"^[\p{L}]+$";
    }
}
