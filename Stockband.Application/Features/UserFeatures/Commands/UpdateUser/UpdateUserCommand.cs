using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateUser;

public class UpdateUserCommand:IRequest<BaseResponse>
{
    public int RequestedUserId { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}