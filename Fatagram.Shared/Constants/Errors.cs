using Fatagram.Shared.Common;

namespace Fatagram.Shared.Constants
{
    public static class Errors
    {
        public static class Auth
        {
            public static readonly Error InvalidCredentials = new(
                "INVALID_CREDENTIALS",
                "The provided credentials are invalid."
            );
            public static readonly Error UsernameExisted = new(
                "USERNAME_EXISTED",
                "Username already exists."
            );
            public static readonly Error EmailExisted = new(
                "EMAIL_EXISTED",
                "Email already exists."
            );
            public static readonly Error UserNotFound = new("USER_NOT_FOUND", "User not found.");
            public static readonly Error AccountNotFound = new(
                "ACCOUNT_NOT_FOUND",
                "Account not found."
            );
            public static readonly Error Unauthorized = new("UNAUTHORIZED", "Unauthorized access.");
            public static readonly Error PasswordIncorrect = new(
                "PASSWORD_INCORRECT",
                "Password is incorrect."
            );
        }

        public static class User
        {
            public static readonly Error NotFound = new("USER_NOT_FOUND", "User not found.");
        }
    }
}
