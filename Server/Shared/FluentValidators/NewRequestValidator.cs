using System.Globalization;
using FluentValidation;
using Shared.DTOs.Request;
using Shared.FluentValidators.Properties;

namespace Shared.FluentValidators;

public class NewRequestValidator : AbstractValidator<NewRequestDTO>
{
    public NewRequestValidator()
    {
        RuleFor(x => x.Email).SetValidator(new EmailValidator());

        RuleFor(x => x.DatetimeEnd)
            .NotEmpty().WithMessage("Datetime end is required.")
            .Must(x => DateTime.TryParse(x, new CultureInfo("it-IT"), DateTimeStyles.None, out _)).WithMessage("Datetime end must be greater than current datetime.");

        RuleFor(x => x.Percentage)
            .GreaterThanOrEqualTo(0).WithMessage("Percentage must be greater than or equal to 0.")
            .LessThanOrEqualTo(100).WithMessage("Percentage must be less than or equal to 100.");

        RuleFor(x => x.PhoneNumber)
            .Matches(@"^(\d{10})?$").WithMessage("Phone number must be 10 digits or empty string. (1234567890 or '')");
    }
}
