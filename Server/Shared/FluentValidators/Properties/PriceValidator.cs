using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class PriceValidator : AbstractValidator<decimal>
{
    public PriceValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .PrecisionScale(5, 2, false).WithMessage("Price must have a precision of 2 digits.");
    }
}
