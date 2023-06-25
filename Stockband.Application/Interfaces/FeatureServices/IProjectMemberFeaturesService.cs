namespace Stockband.Application.Interfaces.FeatureServices;

public interface IProjectMemberFeaturesService
{
    Task<bool> IsProjectMembersLimitExceeded(int projectId);
}