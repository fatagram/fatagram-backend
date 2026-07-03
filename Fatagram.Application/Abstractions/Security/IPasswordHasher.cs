namespace Fatagram.Application.Abstractions.Security
{
    public interface IPasswordHasher
    {
        string HashPassword(string password);
        bool Verify(string password, string hashedPassword);
    }
}
