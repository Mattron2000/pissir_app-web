using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class PriceTypeValidator : AbstractValidator<string>
{
    public PriceTypeValidator()
    {
        RuleFor(x => x)
            .NotEmpty().WithMessage("Price type is required.")
            .Must(x => x == "PARKING" || x == "CHARGING").WithMessage("Price type must be either 'PARKING' or 'CHARGING'.");
    }
}
