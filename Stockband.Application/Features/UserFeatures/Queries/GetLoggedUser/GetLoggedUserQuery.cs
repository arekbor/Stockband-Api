using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;

public class GetLoggedUserQuery:IRequest<BaseResponse<GetLoggedUserQueryViewModel>>
{
    public GetLoggedUserQuery()
    {
        
    }
    public GetLoggedUserQuery(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}