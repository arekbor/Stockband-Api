using FluentValidation;
using Stockband.Application.Common.FluentValidationRuleBuilders;

namespace Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;

public class GetLoggedUserQueryValidator:AbstractValidator<GetLoggedUserQuery>
{
    public GetLoggedUserQueryValidator()
    {
        RuleFor(x => x.Email)
            .EmailUserRuleBuilder();

        RuleFor(x => x.Password)
            .PasswordNotEmptyUserRuleBuilder();
    }
}