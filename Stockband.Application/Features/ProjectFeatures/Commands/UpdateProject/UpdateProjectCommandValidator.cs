using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommandValidator:AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .ProjectNameProjectRule();

        RuleFor(x => x.ProjectDescription)
            .ProjectDescriptionProjectRule();
    }
}