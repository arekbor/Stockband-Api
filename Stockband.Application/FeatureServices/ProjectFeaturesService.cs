using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain.Entities;

namespace Stockband.Application.FeatureServices;

public class ProjectFeaturesService:IProjectFeaturesService
{
    private readonly IConfigurationHelperService _configurationHelperService;
    private readonly IProjectRepository _projectRepository;
    
    public ProjectFeaturesService(
        IConfigurationHelperService configurationHelperService, 
        IProjectRepository projectRepository)
    {
        _configurationHelperService = configurationHelperService;
        _projectRepository = projectRepository;
    }
    public async Task<bool> IsProjectsLimitExceeded(int projectOwnerId)
    {
        int limit = _configurationHelperService.GetProjectsLimitPerUser();

        IEnumerable<Project> projects = await _projectRepository
            .GetAllProjectsByOwnerId(projectOwnerId);

        return (projects.Count() >= limit);
    }

    public async Task<bool> IsProjectNameAlreadyExists(string projectName) => 
        await _projectRepository.GetProjectByNameAsync(projectName) != null;
    
}