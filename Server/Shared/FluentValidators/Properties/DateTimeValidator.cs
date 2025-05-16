using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class DateTimeValidator : AbstractValidator<string>
{
    public DateTimeValidator()
    {
        RuleFor(x => x).NotEmpty().NotNull().WithMessage("Date is required.")
            .Must(x => DateTime.TryParse(x, out _)).WithMessage("Invalid datetime format. (YYYY-MM-DD HH:MM:SS)");
    }
}
