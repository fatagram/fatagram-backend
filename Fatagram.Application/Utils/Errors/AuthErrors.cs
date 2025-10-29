using System;

namespace Fatagram.Application.Utils.Errors
{
    public static class AuthErrors
    {
        public const string TOKEN_INVALID = "TOKEN_INVALID";
        public const string ACCOUNT_NOT_FOUND = "ACCOUNT_NOT_FOUND";
        public const string EMPTY_PASSWORD = "EMPTY_PASSWORD";
        public const string WEAK_PASSWORD = "WEAK_PASSWORD";
        public const string WRONG_PASSWORD = "WRONG_PASSWORD";
        public const string INCORRECT_PASSWORD = "PASSWORD_NOT_CORRECT_FORMAT";
        public const string USERNAME_NOT_EXIST = "USERNAME_NOT_EXIST";
        public const string USERNAME_NOT_CORRECT_FORMAT = "USERNAME_NOT_CORRECT_FORMAT";
        public const string EMPTY_USERNAME = "EMPTY_USERNAME";

        public static string? MapMessage(string errorCode)
        {
            return errorCode switch
            {
                TOKEN_INVALID => "The provided token is invalid.",
                ACCOUNT_NOT_FOUND => "The requested account does not exist.",
                EMPTY_PASSWORD => "Password cannot be empty.",
                WEAK_PASSWORD => "The provided password is too weak.",
                WRONG_PASSWORD => "The provided password is incorrect.",
                INCORRECT_PASSWORD => "Password format is not correct.",
                USERNAME_NOT_EXIST => "The specified username does not exist.",
                USERNAME_NOT_CORRECT_FORMAT => "Username format is not correct.",
                EMPTY_USERNAME => "Username cannot be empty.",
                _ => null,
            };
        }
    }
}
