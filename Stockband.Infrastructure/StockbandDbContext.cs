using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Common;

namespace Stockband.Infrastructure;

public class StockbandDbContext:DbContext
{
    public StockbandDbContext(DbContextOptions<StockbandDbContext> options)
        : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        foreach (EntityEntry<AuditEntity>? entry in ChangeTracker.Entries<AuditEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Deleted:
                {
                    entry.Entity.Deleted = true;
                    entry.Entity.ModifiedAt = DateTime.Now;
                    entry.State = EntityState.Modified;
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
        foreach (IMutableEntityType? entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(AuditEntity).IsAssignableFrom(entityType.ClrType))
            {
                entityType.AddSoftDeleteQueryFilter();
            }
        }
        base.OnModelCreating(modelBuilder);
    }
}