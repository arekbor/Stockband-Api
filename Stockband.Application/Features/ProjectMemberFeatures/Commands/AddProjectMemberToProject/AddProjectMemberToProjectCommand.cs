using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;

public class AddProjectMemberToProjectCommand:IRequest<BaseResponse>
{
    public int ProjectOwnerId { get; set; }
    public int ProjectId { get; set; }
    public int MemberId { get; set; }
}