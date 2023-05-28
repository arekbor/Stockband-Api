using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.Repositories;
namespace Stockband.Infrastructure.Repositories;

public class BaseRepository<T>: IBaseRepository<T>
    where T : class
{
    protected readonly StockbandDbContext _stockbandDbContext;

    public BaseRepository(StockbandDbContext stockbandDbContext)
    {
        _stockbandDbContext = stockbandDbContext;
    }
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _stockbandDbContext.Set<T>().FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _stockbandDbContext.Set<T>().ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _stockbandDbContext.Set<T>().AddAsync(entity);
        await _stockbandDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _stockbandDbContext.Entry(entity).State = EntityState.Modified;
        await _stockbandDbContext.SaveChangesAsync();
    }

    public async  Task DeleteAsync(T entity)
    {
        _stockbandDbContext.Set<T>().Remove(entity);
        await _stockbandDbContext.SaveChangesAsync();
    }
}