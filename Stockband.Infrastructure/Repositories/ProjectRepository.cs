using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Configuration;

namespace Stockband.Infrastructure.Repositories;

public class ProjectRepository: BaseRepository<Project>, IProjectRepository
{
    public ProjectRepository(StockbandDbContext stockbandDbContext) : base(stockbandDbContext)
    {
    }

    public async Task<Project?> GetProjectByNameAsync(string name)
    {
        Project? project = await _stockbandDbContext
            .Projects
            .FirstOrDefaultAsync(x => x.Name == name);

        return project;
    }

    public async Task<IEnumerable<Project>> GetAllProjectsByOwnerId(int ownerId)
    {
        List<Project> projects = await _stockbandDbContext.Projects
            .Where(x => x.OwnerId == ownerId)
            .AsNoTracking()
            .ToListAsync();

        return projects;
    }
}