using Microsoft.EntityFrameworkCore;
using Stockband.Application.Interfaces.MemoryRepositories;
using Stockband.Infrastructure.Configuration;

namespace Stockband.Infrastructure.MemoryRepositories;

public class MemoryBaseRepository<T>: IMemoryBaseRepository<T>
    where T:class
{
    protected readonly StockbandMemoryDbContext _stockbandMemoryDbContext;

    public MemoryBaseRepository(StockbandMemoryDbContext stockbandMemoryDbContext)
    {
        _stockbandMemoryDbContext = stockbandMemoryDbContext;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _stockbandMemoryDbContext.Set<T>().FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _stockbandMemoryDbContext.Set<T>().AddAsync(entity);
        await _stockbandMemoryDbContext.SaveChangesAsync();
    }
    
    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _stockbandMemoryDbContext.Set<T>().AddRangeAsync(entities);
        await _stockbandMemoryDbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _stockbandMemoryDbContext.Entry(entity).State = EntityState.Modified;
        await _stockbandMemoryDbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _stockbandMemoryDbContext.Set<T>().Remove(entity);
        await _stockbandMemoryDbContext.SaveChangesAsync();
    }
}