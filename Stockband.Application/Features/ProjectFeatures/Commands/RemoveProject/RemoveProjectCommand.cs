using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;

public class RemoveProjectCommand:IRequest<BaseResponse>
{
    public RemoveProjectCommand()
    {
        
    }
    
    public RemoveProjectCommand(int projectId)
    {
        ProjectId = projectId;
    }
    public int ProjectId { get; set; }
}