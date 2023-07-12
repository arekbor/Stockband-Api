using MediatR;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;

public class CreateProjectCommand:IRequest<BaseResponse>
{
    public CreateProjectCommand()
    {
        
    }
    public CreateProjectCommand(string projectName, string projectDescription)
    {
        ProjectName = projectName;
        ProjectDescription = projectDescription;
    }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
}