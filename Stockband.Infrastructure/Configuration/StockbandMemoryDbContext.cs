using Microsoft.EntityFrameworkCore;
using Stockband.Domain.MemoryEntities;

namespace Stockband.Infrastructure.Configuration;

public class StockbandMemoryDbContext:DbContext
{
    public StockbandMemoryDbContext(DbContextOptions<StockbandMemoryDbContext> options)
        :base(options)
    {
        
    }
    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }
}