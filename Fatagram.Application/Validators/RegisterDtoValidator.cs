using Fatagram.Application.Dtos.Auth;
using FluentValidation;

namespace Fatagram.Application.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        public RegisterDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithErrorCode("USERNAME_IS_REQUIRED")
                .WithMessage("Username is required.")
                .MaximumLength(20)
                .WithErrorCode("USERNAME_IS_NOT_VALID")
                .WithMessage("Username must be at least 20 characters.")
                .MinimumLength(6)
                .WithErrorCode("USERNAME_IS_NOT_VALID")
                .WithMessage("Username must be at least 6 characters.");

            RuleFor(x => x.Email)
                .NotEmpty()
                .WithErrorCode("EMAIL_IS_REQUIRED")
                .WithMessage("Email is required.")
                .EmailAddress()
                .WithErrorCode("EMAIL_IS_NOT_VALID")
                .WithMessage("Email is not valid.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithErrorCode("PASSWORD_IS_REQUIRED")
                .MinimumLength(8)
                .WithErrorCode("PASSWORD_IS_NOT_VALID")
                .WithMessage("Password must be at least 8 characters.");
        }
    }
}
