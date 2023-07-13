using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator:AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .PasswordNotEmptyUserRule();

        RuleFor(x => x.NewPassword)
            .PasswordUserRule(x => x.ConfirmNewPassword);
    }
}