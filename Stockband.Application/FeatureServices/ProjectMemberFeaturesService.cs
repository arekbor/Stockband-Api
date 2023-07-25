using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Entities;

namespace Stockband.Application.FeatureServices;

public class ProjectMemberFeaturesService:IProjectMemberFeaturesService
{
    private readonly IConfigurationHelperService _configurationHelperService;
    private readonly IProjectMemberRepository _projectMemberRepository;
    
    public ProjectMemberFeaturesService(
        IConfigurationHelperService configurationHelperService, 
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

    public async Task<bool> IsProjectMemberBelongToProject(int projectId, int memberId)
    {
        IEnumerable<ProjectMember> projectMembers = await _projectMemberRepository
            .GetAllProjectMembersByProjectIdAsync(projectId);
        
        return projectMembers.Any(x => x.MemberId == memberId);
    }

    public async Task<bool> IsAnyProjectMemberBelongToProject(int projectId)
    {
        IEnumerable<ProjectMember> projectMembers = await _projectMemberRepository
            .GetAllProjectMembersByProjectIdAsync(projectId);

        return projectMembers.Any();
    }
}