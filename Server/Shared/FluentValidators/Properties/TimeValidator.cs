using FluentValidation;

namespace Shared.FluentValidators.Properties;

public class TimeValidator : AbstractValidator<string>
{
    public TimeValidator()
    {
        RuleFor(x => x).NotEmpty().NotNull().WithMessage("Time is required.")
            .Must(x => TimeOnly.TryParse(x, out _)).WithMessage("Invalid time format. (HH:mm)");
    }
}
