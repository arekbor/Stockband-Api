namespace Stockband.Application.Interfaces.MemoryRepositories;

public interface IMemoryBaseRepository<T>
    where T: class
{
    Task<T?> GetByIdAsync(int id);
    Task AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}