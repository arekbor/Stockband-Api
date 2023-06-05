using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;

public class RemoveMemberFromProjectCommand:IRequest<BaseResponse>
{
    public int RequestedUserId { get; set; }
    public int ProjectId { get; set; }
    public int MemberId { get; set; }
}