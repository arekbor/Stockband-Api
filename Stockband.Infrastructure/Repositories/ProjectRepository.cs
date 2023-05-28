using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;

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

    public async Task<Project?> GetProjectByIdWithIncludedUserAsync(int id)
    {
        Project? project = await _stockbandDbContext
            .Projects
            .AsNoTracking()
            .Include(x => x.Owner)
            .FirstOrDefaultAsync(x => x.Id == id);

        return project;
    }
}