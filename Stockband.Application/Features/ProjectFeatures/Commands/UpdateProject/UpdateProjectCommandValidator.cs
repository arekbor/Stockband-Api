using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommandValidator:AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .ProjectNameProjectRuleBuilder();

        RuleFor(x => x.ProjectDescription)
            .ProjectDescriptionProjectRuleBuilder();
    }
}