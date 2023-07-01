using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator:AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .PasswordNotEmptyUserRuleBuilder();

        RuleFor(x => x.NewPassword)
            .PasswordUserRuleBuilder(x => x.ConfirmNewPassword);
    }
}