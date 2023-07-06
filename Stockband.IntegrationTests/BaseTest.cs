using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stockband.Infrastructure;

namespace Stockband.IntegrationTests;

public abstract class BaseTest
{
    protected StockbandDbContext Context = null!;
    
    [SetUp]
    protected async Task Setup()
    {
        DbContextOptions<StockbandDbContext> dbContextOptions = GetDbContextOptions();
        Context = new StockbandDbContext(dbContextOptions);
        await Context.Database.EnsureCreatedAsync();
    }

    [TearDown]
    protected async Task CleanUp()
    {
        await Context.Database.EnsureDeletedAsync();
        await Context.DisposeAsync();
    }
    
    private static DbContextOptions<StockbandDbContext> GetDbContextOptions()
    {
        DbContextOptions<StockbandDbContext> dbContextOptions =
            new DbContextOptionsBuilder<StockbandDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return dbContextOptions;
    } 
}