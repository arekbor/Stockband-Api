using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Queries.LoginUser;

public class LoginUserQuery:IRequest<BaseResponse<LoginUserQueryViewModel>>
{
    public LoginUserQuery()
    {
        
    }
    public LoginUserQuery(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}