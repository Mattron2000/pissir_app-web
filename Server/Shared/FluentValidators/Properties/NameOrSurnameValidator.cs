using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class NameOrSurnameValidator : AbstractValidator<string>
{
    public NameOrSurnameValidator(string name)
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage(name + " is required.")
            .Matches(@"^[A-Z][a-z]+$").WithMessage(name + " must start with an uppercase letter and contain only valid characters.")
            .MaximumLength(50).WithMessage(name + " can't be longer than 50 characters.");
    }
}
