using FluentValidation;

namespace Stockband.Application.Common.FluentValidationRuleBuilders;

public static partial class RuleBuilders
{
    public static IRuleBuilderOptions<T, string> ProjectNameProjectRuleBuilder<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .StringNotEmptyBaseRuleBuilder()
            .MaximumLength(100).WithMessage("{PropertyName} length must not exceed 100.");
    }

    public static IRuleBuilderOptions<T, string> ProjectDescriptionProjectRuleBuilder<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(1000).WithMessage("{PropertyName} length must not exceed 1000.");
    }
}