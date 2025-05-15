using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class UserTypeValidator : AbstractValidator<string>
{
    public UserTypeValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("User type is required.")
            .Must(x => x == "PREMIUM" || x == "BASE").WithMessage("User type must be either 'PREMIUM' or 'BASE'.");
    }
}
