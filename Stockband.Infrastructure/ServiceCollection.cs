using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Infrastructure.Configuration;
using Stockband.Infrastructure.Repositories;
using Stockband.Infrastructure.Services;

namespace Stockband.Infrastructure;

public static class ServiceCollection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfiguration configuration)
    {
        ConfigurationHelperService config = new ConfigurationHelperService(configuration); 
        
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        services.AddDbContext<StockbandDbContext>(options =>
        {
            options.UseNpgsql(configuration["DefaultConnection"], o =>
                o.SetPostgresVersion(9, 6));
        });
        services.AddDbContext<StockbandMemoryDbContext>(options =>
        {
            options.UseInMemoryDatabase("StockbandInMemory");
        });
        
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                
                ValidIssuer = config.GetAccessTokenIssuer(),
                ValidAudience = config.GetAccessTokenAudience(),
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(config.GetAccessTokenPrivateKey())),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };
        });

        services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IProjectMemberRepository, ProjectMemberRepository>();
        services.AddSingleton<IConfigurationHelperService, ConfigurationHelperService>();
        services.AddSingleton<IAuthenticationUserService, AuthenticationUserService>();
        
        return services;
    }
}