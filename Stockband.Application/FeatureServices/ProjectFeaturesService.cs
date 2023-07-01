using Stockband.Application.Interfaces.Common;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Domain.Entities;

namespace Stockband.Application.FeatureServices;

public class ProjectFeaturesService:IProjectFeaturesService
{
    private readonly IConfigurationHelperCommonService _configurationHelperService;
    private readonly IProjectRepository _projectRepository;
    
    public ProjectFeaturesService(
        IConfigurationHelperCommonService configurationHelperService, 
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
}