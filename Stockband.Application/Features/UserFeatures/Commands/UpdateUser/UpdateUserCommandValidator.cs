using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateUser;

public class UpdateUserCommandValidator:AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailUserRuleBuilder();

        RuleFor(x => x.Username)
            .UsernameUserRuleBuilder();
    }
}