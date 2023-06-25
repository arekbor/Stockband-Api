using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Stockband.Application.CommonServices;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.FeatureServices;
using Stockband.Application.Interfaces.CommonServices;

namespace Stockband.Application;

public static class ServiceCollection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IConfigurationHelperCommonService, ConfigurationHelperCommonService>();
        
        services.AddScoped<IProjectFeaturesService, ProjectFeaturesService>();
        services.AddScoped<IProjectMemberFeaturesService, ProjectMemberFeaturesService>();

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));
        
        return services;
    }
}