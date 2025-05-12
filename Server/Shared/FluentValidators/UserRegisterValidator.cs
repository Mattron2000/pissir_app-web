using FluentValidation;
using Shared.DTOs.User;

namespace Shared.FluentValidators;

public class UserRegisterValidator : AbstractValidator<UserRegisterDTO>
{
    public UserRegisterValidator()
    {
        RuleFor(u => u.Email).SetValidator(new EmailValidator());
        RuleFor(u => u.Password).SetValidator(new PasswordValidator());
        RuleFor(u => u.Name).SetValidator(new NameOrSurnameValidator("Name"));
        RuleFor(u => u.Surname).SetValidator(new NameOrSurnameValidator("Surname"));
    }
}
