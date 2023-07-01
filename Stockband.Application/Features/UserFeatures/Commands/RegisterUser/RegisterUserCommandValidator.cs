using FluentValidation;

namespace Stockband.Application.Features.UserFeatures.Commands.RegisterUser;

public class RegisterUserCommandValidator:AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        //TODO: https://docs.fluentvalidation.net/en/latest/rulesets.html
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MinimumLength(8).WithMessage("{PropertyName} length must be at least 8.")
            .MaximumLength(26).WithMessage("{PropertyName} length must not exceed 26.")
            .Matches(@"[A-Z]+").WithMessage("{PropertyName} must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("{PropertyName} must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("{PropertyName} must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("{PropertyName} must contain at least one (!? *.).")
            .Equal(x => x.ConfirmPassword).WithMessage("Passwords do not match");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .EmailAddress().WithMessage("{PropertyName} is invalid");
        
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required")
            .MinimumLength(5).WithMessage("{PropertyName} length must be at least 5.")
            .MaximumLength(32).WithMessage("{PropertyName} length must not exceed 32.");
    }
}