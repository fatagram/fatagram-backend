namespace Fatagram.Domain.Enums
{
    public enum RegisterResult
    {
        Success,
        UsernameAlreadyExists,
        EmailAlreadyExists,
        PasswordTooWeak,
        UnknownError
    }
}
