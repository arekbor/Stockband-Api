using MediatR;
using Stockband.Domain;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommand:IRequest<BaseResponse>
{
    public UpdateProjectCommand()
    {
        
    }
    
    public UpdateProjectCommand(int projectId, string projectName, string projectDescription)
    {
        ProjectId = projectId;
        ProjectName = projectName;
        ProjectDescription = projectDescription;
    }
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
}