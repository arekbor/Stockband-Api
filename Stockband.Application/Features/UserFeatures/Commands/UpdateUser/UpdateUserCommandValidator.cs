using FluentValidation;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateUser;

public class UpdateUserCommandValidator:AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
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