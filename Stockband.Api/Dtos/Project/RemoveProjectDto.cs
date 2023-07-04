namespace Stockband.Api.Dtos.Project;

public class RemoveProjectDto
{
    public RemoveProjectDto()
    {
        
    }

    public RemoveProjectDto(int projectId)
    {
        ProjectId = projectId;
    }
    
    public int ProjectId { get; set; }
}