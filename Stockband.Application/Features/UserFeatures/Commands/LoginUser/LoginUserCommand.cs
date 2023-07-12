using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.LoginUser;

public class LoginUserCommand:IRequest<BaseResponse>
{
    public LoginUserCommand()
    {
        
    }
    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
    public string Email { get; set; }
    public string Password { get; set; }
}