using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateUser;

public class UpdateUserCommandValidator:AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .EmailUserRule();

        RuleFor(x => x.Username)
            .UsernameUserRule();
    }
}