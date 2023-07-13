using System.Linq.Expressions;
using FluentValidation;

namespace Stockband.Application.Common;

public static class FluentValidationRules
{
    public static IRuleBuilderOptions<T, string> ProjectNameProjectRule<T>
        (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .StringNotEmptyBaseRule()
            .MaximumLength(100).WithMessage("{PropertyName} length must not exceed 100.");
    }

    public static IRuleBuilderOptions<T, string> ProjectDescriptionProjectRule<T>
        (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(1000).WithMessage("{PropertyName} length must not exceed 1000.");
    }
    
    public static IRuleBuilderOptions<T, string> PasswordUserRule<T>
        (this IRuleBuilder<T, string> ruleBuilder, Expression<Func<T, string>> confirmPassword)
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

    public static IRuleBuilderOptions<T, string> EmailUserRule<T>
        (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .EmailAddress().WithMessage("Email scheme is invalid.");
    }
    
    public static IRuleBuilderOptions<T, string> UsernameUserRule<T>
        (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .MinimumLength(5).WithMessage("{PropertyName} length must be at least 5.")
            .MaximumLength(32).WithMessage("{PropertyName} length must not exceed 32.");
    }
    
    public static IRuleBuilderOptions<T, string> PasswordNotEmptyUserRule<T>
        (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .StringNotEmptyBaseRule();
    }
    
    private static IRuleBuilderOptions<T, string> StringNotEmptyBaseRule<T>
        (this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("{PropertyName} is required.")
            .NotNull().WithMessage("{PropertyName} is required.");
    }
}