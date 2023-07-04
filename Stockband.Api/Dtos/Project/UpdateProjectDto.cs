namespace Stockband.Api.Dtos.Project;

public class UpdateProjectDto
{
    public UpdateProjectDto()
    {
        
    }

    public UpdateProjectDto(int projectId, string projectName, string projectDescription)
    {
        ProjectId = projectId;
        ProjectName = projectName;
        ProjectDescription = projectDescription;
    }
    
    public int ProjectId { get; set; }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
}