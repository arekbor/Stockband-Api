using FluentValidation;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommandValidator:AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .MaximumLength(100).WithMessage("{PropertyName} length must not exceed 100.")
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required");
        
        RuleFor(x => x.ProjectDescription)
            .MaximumLength(1000).WithMessage("{PropertyName} length must not exceed 1000.");
    }
}