using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateUser;

public class UpdateUserCommand:IRequest<BaseResponse>
{
    public UpdateUserCommand()
    {
        
    }
    public UpdateUserCommand(int userId, string username, string email)
    {
        UserId = userId;
        Username = username;
        Email = email;
    }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}