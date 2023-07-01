using FluentValidation;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateRole;

public class UpdateRoleCommandValidator:AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleCommandValidator()
    {
        RuleFor(x => x.Role)
            .IsInEnum();
    }
}