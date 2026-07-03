namespace Fatagram.Application.Dtos.User
{
    using Fatagram.Domain.Enums;

    public record OnboardingDefaultDataDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }
        public string? Avatar { get; set; }
        public string? Email { get; set; }
        public Gender? Gender { get; set; }
        public DateTime? BirthDay { get; set; }
    }
}
