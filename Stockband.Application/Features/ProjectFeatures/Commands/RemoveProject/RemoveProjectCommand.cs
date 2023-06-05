using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;

public class RemoveProjectCommand:IRequest<BaseResponse>
{
    public int RequestedUserId { get; set; }
    public int ProjectId { get; set; }
}