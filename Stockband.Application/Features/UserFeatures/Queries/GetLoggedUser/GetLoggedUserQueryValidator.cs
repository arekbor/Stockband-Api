using FluentValidation;

namespace Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;

public class GetLoggedUserQueryValidator:AbstractValidator<GetLoggedUserQuery>
{
    public GetLoggedUserQueryValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("{PropertyName} is invalid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("{PropertyName} is required")
            .NotNull().WithMessage("{PropertyName} is required");
    }
}