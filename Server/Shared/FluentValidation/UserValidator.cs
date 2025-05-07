using FluentValidation;
using Shared.DTOs;

namespace Shared.FluentValidation;

public class UserValidator : AbstractValidator<UserRegisterDTO>
{
    public UserValidator()
    {
        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(u => u.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters.")
            .Matches(@"[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"[a-z]").WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"\d").WithMessage("Password must contain at least one digit.");

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Matches(@"^[A-Z][a-z]+$").WithMessage("Name must start with an uppercase letter and contain only valid characters.")
            .MaximumLength(50).WithMessage("Name can't be longer than 50 characters.");

        RuleFor(u => u.Surname)
            .NotEmpty().WithMessage("Surname is required.")
            .Matches(@"^[A-Z][a-z]+$").WithMessage("Surname must start with an uppercase letter and contain only valid characters.")
            .MaximumLength(50).WithMessage("Surname can't be longer than 50 characters.");
    }
}
