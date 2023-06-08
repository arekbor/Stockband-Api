using MediatR;
using Stockband.Domain;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateRole;

public class UpdateRoleCommand:IRequest<BaseResponse>
{
    public int RequestedUserId { get; set; }
    public int UserId { get; set; }
    public UserRole Role { get; set; }
}