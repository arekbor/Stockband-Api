using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;

public class CreateProjectCommandValidator:AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.ProjectName)
            .ProjectNameProjectRuleBuilder();
        
        RuleFor(x => x.ProjectDescription)
            .ProjectDescriptionProjectRuleBuilder();
    }
}