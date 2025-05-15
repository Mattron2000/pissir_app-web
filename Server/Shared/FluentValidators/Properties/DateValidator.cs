using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class DateValidator : AbstractValidator<string>
{
    public DateValidator()
    {
        RuleFor(x => x).NotEmpty().NotNull().WithMessage("Date is required.")
            .Must(x => DateOnly.TryParse(x, out _)).WithMessage("Invalid date format. (YYYY-MM-DD)");
    }
}
