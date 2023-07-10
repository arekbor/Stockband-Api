using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.UserFeatures.Commands.LoginUser;

public class LoginUserCommandValidator:AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailUserRuleBuilder();

        RuleFor(x => x.Password)
            .PasswordNotEmptyUserRuleBuilder();
    }
}