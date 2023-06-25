using Stockband.Application.Interfaces.CommonServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Domain.Entities;

namespace Stockband.Application.FeatureServices;

public class ProjectMemberFeaturesService:IProjectMemberFeaturesService
{
    private readonly IConfigurationHelperCommonService _configurationHelperService;
    private readonly IProjectMemberRepository _projectMemberRepository;
    
    public ProjectMemberFeaturesService(
        IConfigurationHelperCommonService configurationHelperService, 
        IProjectMemberRepository projectMemberRepository)
    {
        _configurationHelperService = configurationHelperService;
        _projectMemberRepository = projectMemberRepository;
    }

    public async Task<bool> IsProjectMembersLimitExceeded(int projectId)
    {
        int limit = _configurationHelperService.GetProjectMembersLimitPerProject();
        
        IEnumerable<ProjectMember> members = await _projectMemberRepository
            .GetAllProjectMembersByProjectIdAsync(projectId);
        
        return (members.Count() >= limit);
    }
}