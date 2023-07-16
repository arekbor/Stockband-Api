using FluentValidation;
using Stockband.Application.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.LoginUser;

public class LoginUserQueryValidator:AbstractValidator<LoginUserQuery>
{
    public LoginUserQueryValidator()
    {
        RuleFor(x => x.Email)
            .EmailUserRule();

        RuleFor(x => x.Password)
            .PasswordNotEmptyUserRule();
    }
}