using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Infrastructure;

public static class ServiceCollection
{
    public static IServiceCollection AddInfrastructureDbContexts(this IServiceCollection services,
        IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<StockbandDbContext>(options =>
        {
            options.UseNpgsql(configuration["DefaultConnection"], o =>
                o.SetPostgresVersion(9, 6));
        });
        
        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        
        return services;
    }
}