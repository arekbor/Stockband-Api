namespace Stockband.Application.Features.ProjectFeatures.Queries.GetAllUserProjects;

public class GetAllUserProjectsQueryViewModel
{
    public int OwnerId { get; set; }
    public int ProjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}