using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;

namespace Stockband.Infrastructure;

public class StockbandDbContext:DbContext
{
    public StockbandDbContext(DbContextOptions<StockbandDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (EntityEntry<AuditEntity>? entry in ChangeTracker.Entries<AuditEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                {
                    entry.Entity.CreatedAt = DateTime.Now;
                    break;
                }
                case EntityState.Modified:
                {
                    entry.Entity.ModifiedAt = DateTime.Now;
                    break;
                }
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockbandDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}