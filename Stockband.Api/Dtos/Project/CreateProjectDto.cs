namespace Stockband.Api.Dtos.Project;

public class CreateProjectDto
{
    public CreateProjectDto()
    {
        
    }

    public CreateProjectDto(string projectName, string projectDescription)
    {
        ProjectName = projectName;
        ProjectDescription = projectDescription;
    }
    public string ProjectName { get; set; }
    public string ProjectDescription { get; set; }
}