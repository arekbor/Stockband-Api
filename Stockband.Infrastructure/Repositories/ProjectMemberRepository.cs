using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Configuration;

namespace Stockband.Infrastructure.Repositories;

public class ProjectMemberRepository : BaseRepository<ProjectMember>, IProjectMemberRepository
{
    public ProjectMemberRepository(StockbandDbContext stockbandDbContext) : base(stockbandDbContext)
    {
    }

    public async Task<IEnumerable<ProjectMember>> GetAllProjectMembersByProjectIdAsync(int projectId)
    {
        return await _stockbandDbContext
            .ProjectMembers
            .Where(x => x.ProjectId == projectId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ProjectMember?> GetProjectMemberIncludedProjectAndMemberAsync(int projectId, int memberId)
    {
        return await _stockbandDbContext
            .ProjectMembers
            .Include(x => x.Project)
            .Include(x => x.Member)
            .FirstOrDefaultAsync(x => x.ProjectId == projectId && x.MemberId == memberId);
    }
}