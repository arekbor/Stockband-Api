using FluentValidation;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator:AbstractValidator<UpdatePasswordCommand>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MinimumLength(8).WithMessage("{PropertyName} length must be at least 8.")
            .MaximumLength(26).WithMessage("{PropertyName} length must not exceed 26.")
            .Matches(@"[A-Z]+").WithMessage("{PropertyName} must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("{PropertyName} must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("{PropertyName} must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("{PropertyName} must contain at least one (!? *.).")
            .Equal(x => x.ConfirmNewPassword).WithMessage("Passwords do not match");
    }
}