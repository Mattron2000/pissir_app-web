using FluentValidation;
using Shared.DTOs.User;
using Shared.FluentValidators.Properties;

namespace Shared.FluentValidators;

public class UserLoginValidator : AbstractValidator<UserLoginDTO>
{
    public UserLoginValidator()
    {
        RuleFor(x => x.Email).SetValidator(new EmailValidator());
        RuleFor(x => x.Password).SetValidator(new PasswordValidator());
    }
}
