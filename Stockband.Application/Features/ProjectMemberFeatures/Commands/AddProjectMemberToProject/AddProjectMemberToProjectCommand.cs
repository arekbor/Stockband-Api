using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;

public class AddProjectMemberToProjectCommand:IRequest<BaseResponse>
{
    public AddProjectMemberToProjectCommand()
    {
        
    }
    public AddProjectMemberToProjectCommand(int projectId, int memberId)
    {
        ProjectId = projectId;
        MemberId = memberId;
    }
    public int ProjectId { get; set; }
    public int MemberId { get; set; }
}