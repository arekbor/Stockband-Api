using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;

public class CreateProjectCommandValidator:AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .ProjectNameProjectRule();
        
        RuleFor(x => x.ProjectDescription)
            .ProjectDescriptionProjectRule();
    }
}