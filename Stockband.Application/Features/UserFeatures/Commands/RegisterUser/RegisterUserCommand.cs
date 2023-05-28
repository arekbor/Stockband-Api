using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.UserFeatures.Commands.RegisterUser;

public class RegisterUserCommand:IRequest<BaseResponse>
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
}