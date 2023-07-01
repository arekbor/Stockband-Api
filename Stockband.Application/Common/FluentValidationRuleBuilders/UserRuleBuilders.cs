using System.Linq.Expressions;
using FluentValidation;

namespace Stockband.Application.Common.FluentValidationRuleBuilders;

public static partial class RuleBuilders
{
    public static IRuleBuilderOptions<T, string> PasswordUserRuleBuilder<T>(this IRuleBuilder<T, string> ruleBuilder, Expression<Func<T, string>> confirmPassword)
    {
        return ruleBuilder
            .MinimumLength(8).WithMessage("{PropertyName} length must be at least 8.")
            .MaximumLength(26).WithMessage("{PropertyName} length must not exceed 26.")
            .Matches(@"[A-Z]+").WithMessage("{PropertyName} must contain at least one uppercase letter.")
            .Matches(@"[a-z]+").WithMessage("{PropertyName} must contain at least one lowercase letter.")
            .Matches(@"[0-9]+").WithMessage("{PropertyName} must contain at least one number.")
            .Matches(@"[\!\?\*\.]+").WithMessage("{PropertyName} must contain at least one (!? *.).")
            .Equal(confirmPassword).WithMessage("Passwords do not match");
    }

    public static IRuleBuilderOptions<T, string> EmailUserRuleBuilder<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .EmailAddress().WithMessage("Email scheme is invalid.");
    }
    
    public static IRuleBuilderOptions<T, string> UsernameUserRuleBuilder<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(5).WithMessage("{PropertyName} length must be at least 5.")
            .MaximumLength(32).WithMessage("{PropertyName} length must not exceed 32.");
    }
    
    public static IRuleBuilderOptions<T, string> PasswordNotEmptyUserRuleBuilder<T>(
        this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .StringNotEmptyBaseRuleBuilder();
    }
}