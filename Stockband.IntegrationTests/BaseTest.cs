using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stockband.Infrastructure.Configuration;

namespace Stockband.IntegrationTests;

public abstract class BaseTest
{
    protected StockbandDbContext StockbandDbContext = null!;
    protected StockbandMemoryDbContext StockbandMemoryDbContext = null!;
    
    [SetUp]
    protected async Task Setup()
    {
        DbContextOptions<StockbandDbContext> stockbandDbContext = GetStockbandDbContextOptions();
        StockbandDbContext = new StockbandDbContext(stockbandDbContext);
        await StockbandDbContext.Database.EnsureCreatedAsync();

        DbContextOptions<StockbandMemoryDbContext> stockbandInMemoryDbContext = GetStockbandInMemoryDbContextOptions();
        
        StockbandMemoryDbContext = new StockbandMemoryDbContext(stockbandInMemoryDbContext);
        await StockbandMemoryDbContext.Database.EnsureCreatedAsync();
    }

    [TearDown]
    protected async Task CleanUp()
    {
        await StockbandDbContext.Database.EnsureDeletedAsync();
        await StockbandDbContext.DisposeAsync();

        await StockbandMemoryDbContext.Database.EnsureCreatedAsync();
        await StockbandMemoryDbContext.DisposeAsync();
    }
    
    private static DbContextOptions<StockbandDbContext> GetStockbandDbContextOptions()
    {
        return new DbContextOptionsBuilder<StockbandDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
    } 
    
    private static DbContextOptions<StockbandMemoryDbContext> GetStockbandInMemoryDbContextOptions()
    {
        return new DbContextOptionsBuilder<StockbandMemoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
    } 
}