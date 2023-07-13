using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.RegisterUser;

public class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Password)
            .PasswordUserRule(x => x.ConfirmPassword);

        RuleFor(x => x.Email)
            .EmailUserRule();

        RuleFor(x => x.Username)
            .UsernameUserRule();
    }
}