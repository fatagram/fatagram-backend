using Fatagram.Shared.Common;

namespace Fatagram.Shared.Constants
{
    public static class Errors
    {
        public static class Auth
        {
            public static readonly Error InvalidCredentials = new(
                "Auth.InvalidCredentials",
                "The provided credentials are invalid."
            );
            public static readonly Error UsernameExisted = new(
                "Auth.UsernameExisted",
                "Username already exists."
            );
            public static readonly Error EmailExisted = new(
                "Auth.EmailExisted",
                "Email already exists."
            );
            public static readonly Error UserNotFound = new("Auth.UserNotFound", "User not found.");
            public static readonly Error AccountNotFound = new(
                "Auth.AccountNotFound",
                "Account not found."
            );
            public static readonly Error Unauthorized = new(
                "Auth.Unauthorized",
                "Unauthorized access."
            );
        }

        public static class User
        {
            public static readonly Error NotFound = new("User.NotFound", "User not found.");
        }
    }
}
