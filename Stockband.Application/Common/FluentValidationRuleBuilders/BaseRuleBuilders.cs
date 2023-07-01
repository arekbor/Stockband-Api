using FluentValidation;

namespace Stockband.Application.Common.FluentValidationRuleBuilders;

public static partial class RuleBuilders
{
    private static IRuleBuilderOptions<T, string> StringNotEmptyBaseRuleBuilder<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required");
    }
}