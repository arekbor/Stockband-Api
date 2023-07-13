using MediatR;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateRole;

public class UpdateRoleCommand:IRequest<BaseResponse>
{
    public UpdateRoleCommand()
    {
        
    }
    public UpdateRoleCommand(int userId, UserRole role)
    {
        UserId = userId;
        Role = role;
    }
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}