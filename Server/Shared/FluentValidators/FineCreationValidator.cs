using FluentValidation;
using Shared.DTOs.Fine;
using Shared.FluentValidators.Properties;

namespace Shared.FluentValidators;

public class FineCreationValidator : AbstractValidator<FineNewDTO>
{
    public FineCreationValidator()
    {
        RuleFor(x => x.Email).SetValidator(new EmailValidator());
        RuleFor(x => x.DatetimeStart).SetValidator(new DateTimeValidator());
        RuleFor(x => x.DatetimeEnd).SetValidator(new DateTimeValidator());
    }
}
