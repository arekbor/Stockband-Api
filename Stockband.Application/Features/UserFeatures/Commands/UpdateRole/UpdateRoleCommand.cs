using MediatR;
using Stockband.Application.Common.Attributes;
using Stockband.Domain;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateRole;

[AllowRole(UserRole.Admin)]
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