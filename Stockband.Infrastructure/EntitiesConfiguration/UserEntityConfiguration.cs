using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Stockband.Domain.Entities;

namespace Stockband.Infrastructure.EntitiesConfiguration;

public class UserEntityConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(x => x.Username)
            .IsRequired()
            .HasMaxLength(60);

        builder
            .HasIndex(x => x.Username)
            .IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(60);

        builder.Property(x => x.Password)
            .IsRequired()
            .HasMaxLength(260);
    }
}