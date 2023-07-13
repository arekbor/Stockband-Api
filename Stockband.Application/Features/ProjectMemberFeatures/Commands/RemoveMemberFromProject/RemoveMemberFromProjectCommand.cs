using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;

public class RemoveMemberFromProjectCommand:IRequest<BaseResponse>
{
    public RemoveMemberFromProjectCommand(int projectId, int memberId)
    {
        ProjectId = projectId;
        MemberId = memberId;
    }
    public int ProjectId { get; set; }
    public int MemberId { get; set; }
}