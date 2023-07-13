namespace Stockband.Application.Interfaces.FeatureServices;

public interface IProjectMemberFeaturesService
{
    Task<bool> IsProjectMembersLimitExceeded(int projectId);
    Task<bool> IsProjectMemberBelongToProject(int projectId, int memberId);
    Task<bool> IsAnyProjectMemberBelongToProject(int projectId);
}