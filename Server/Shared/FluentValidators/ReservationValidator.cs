using FluentValidation;
using Shared.DTOs.Reservation;
using Shared.FluentValidators.Properties;

namespace Shared.FluentValidators;

public class ReservationValidator : AbstractValidator<ReservationCreateDTO>
{
    public ReservationValidator()
    {
        RuleFor(x => x.Email).SetValidator(new EmailValidator());
        RuleFor(x => x.SlotId)
            .NotEmpty().WithMessage("Slot Id is required.")
            .GreaterThan(0).WithMessage("Slot Id must be greater than 0.");

        RuleFor(x => x.DatetimeStart)
            .NotEmpty().WithMessage("Datetime start is required.")
            .GreaterThan(DateTime.Now).WithMessage("Datetime start must be greater than now.");

        RuleFor(x => x.DatetimeEnd)
            .NotEmpty().WithMessage("Datetime end is required.")
            .GreaterThan(x => x.DatetimeStart).WithMessage("Datetime end must be greater than datetime start.");
    }
}
