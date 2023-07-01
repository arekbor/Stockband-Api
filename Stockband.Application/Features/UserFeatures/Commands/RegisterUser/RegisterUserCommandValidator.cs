using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.UserFeatures.Commands.RegisterUser;

public class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Password)
            .PasswordUserRuleBuilder(x => x.ConfirmPassword);

        RuleFor(x => x.Email)
            .EmailUserRuleBuilder();

        RuleFor(x => x.Username)
            .UsernameUserRuleBuilder();
    }
}