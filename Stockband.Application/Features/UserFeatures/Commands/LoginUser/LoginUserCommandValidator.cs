using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.LoginUser;

public class LoginUserCommandValidator:AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailUserRule();

        RuleFor(x => x.Password)
            .PasswordNotEmptyUserRule();
    }
}