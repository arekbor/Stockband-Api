using Stockband.Domain.Entities;

namespace Stockband.Application.Interfaces.Repositories;

public interface IProjectMemberRepository: IBaseRepository<ProjectMember>
{
    Task<IEnumerable<ProjectMember>> GetAllProjectMembersByProjectIdAsync(int projectId);
    Task<ProjectMember?> GetProjectMemberIncludedProjectAndMemberAsync(int projectId, int memberId);
}