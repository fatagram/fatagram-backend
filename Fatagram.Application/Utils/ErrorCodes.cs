using System.Runtime.CompilerServices;

namespace Fatagram.Application.Utils
{
    /// <summary>
    /// Error codes
    /// </summary>
    public static class ErrorCodes
    {
        #region Error codes for register

        /// <summary>
        /// Register: Create user failed
        /// </summary>
        public const string REGISTER_USERNAME_EXISTED = "REGISTER_USERNAME_EXISTED";

        /// <summary>
        /// Register: Last name required
        /// </summary>
        public const string REGISTER_LAST_NAME_REQUIRED = "LAST_NAME_REQUIRED";

        /// <summary>
        /// Register: First name required
        /// </summary>
        public const string REGISTER_FIRST_NAME_REQUIRED = "FIRST_NAME_REQUIRED";

        /// <summary>
        /// Register: Email required
        /// </summary>
        public const string REGISTER_FAILED = "REGISTER_FAILED";
       

        #endregion

        #region Error codes for login

        /// <summary>
        /// Login: Username not found
        /// </summary>
        public const string LOGIN_USERNAME_NOT_FOUND = "LOGIN_USERNAME_NOT_FOUND";

        #endregion

        #region Error codes for JWT Token

        /// <summary>
        /// Token invalid
        /// </summary>
        public const string ACCESS_TOKEN_INVALID = "TOKEN_INVALID";

        #endregion

        #region Error code for refresh token

        /// <summary>
        /// Refresh token invalid
        /// </summary>
        public const string REFRESH_TOKEN_INVALID = "REFRESH_TOKEN_INVALID";
       

        /// <summary>
        /// Refresh token expired
        /// </summary>
        public const string REFRESH_TOKEN_EXPIRED = "REFRESH_TOKEN_EXPIRED";


        /// <summary>
        /// Refresh token deleted
        /// </summary>
        public const string REFRESH_TOKEN_DELETE_FAILED = "REFRESH_TOKEN_DELETED_FAILED";

        #endregion

        #region Error code for account

        /// <summary>
        /// Account not found
        /// </summary>
        public const string ACCOUNT_NOT_FOUND = "ACCOUNT_NOT_FOUND";

        #endregion

        #region Error code for user

        /// <summary>
        /// Update user failed
        /// </summary>
        public const string UPDATE_USER_FAILED = "UPDATE_USER_FAILED";

        /// <summary>
        /// Bio too long
        /// </summary>
        public const string BIO_TOO_LONG = "BIO_TOO_LONG";

        /// <summary>
        /// Description too long
        /// </summary>
        public const string DESCRIPTION_TOO_LONG = "DESCRIPTION_TOO_LONG";

        #endregion

        #region General error codes


        /// <summary>
        /// Register: Password is not correct format
        /// </summary>

        public const string USERNAME_NOT_CORRECT_FORMAT = "USERNAME_NOT_CORRECT_FORMAT";


        /// <summary>
        /// Register: Email is not correct format
        /// </summary>
        public const string EMAIL_NOT_CORRECT_FORMAT = "EMAIL_NOT_CORRECT_FORMAT";


        /// <summary>
        /// Register: Phone number is not correct format
        /// </summary>
        public const string PHONE_NUMBER_NOT_CORRECT_FORMAT = "PHONE_NUMBER_NOT_CORRECT_FORMAT";


        /// <summary>
        /// Register: Last name is not correct format
        /// </summary>
        public const string LASTNAME_NOT_CORRECT_FORMAT = "LASTNAME_NOT_CORRECT_FORMAT";



        /// <summary>
        /// Register: First name is not correct format
        /// </summary>
        public const string FIRSTNAME_NOT_CORRECT_FORMAT = "FIRSTNAME_NOT_CORRECT_FORMAT";

        /// <summary>
        /// Register: Phone number is not correct format
        /// </summary>
        public const string PASSWORD_NOT_CORRECT_FORMAT = "PASSWORD_NOT_CORRECT_FORMAT";


        /// <summary>
        /// Login: Create user failed
        /// </summary>
        public const string WRONG_PASSWORD = "WRONG_PASSWORD";

        /// <summary>
        /// Invalid input
        /// </summary>
        public const string INVALID_INPUT = "INVALID_INPUT";


        public const string EMAIL_EXISTED = "EMAIL_EXISTED";

        #endregion

    }
}
