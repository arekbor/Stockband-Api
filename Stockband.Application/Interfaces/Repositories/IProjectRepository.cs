using Stockband.Domain.Entities;

namespace Stockband.Application.Interfaces.Repositories;

public interface IProjectRepository : IBaseRepository<Project>
{
    Task<Project?> GetProjectByNameAsync(string name);

    Task<IEnumerable<Project>> GetAllProjectsByOwnerId(int ownerId);
}