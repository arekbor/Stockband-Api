using MediatR;
using Stockband.Application.Common.Attributes;
using Stockband.Domain;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateRole;

[AllowRole(UserRole.Admin)]
public class UpdateRoleCommand:IRequest<BaseResponse>
{
    public int RequestedUserId { get; set; }
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}