using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Stockband.Application.Behaviors;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.FeatureServices;

namespace Stockband.Application;

public static class ServiceCollection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectFeaturesService, ProjectFeaturesService>();
        services.AddScoped<IProjectMemberFeaturesService, ProjectMemberFeaturesService>();
        services.AddScoped<IUserFeaturesService, UserFeaturesService>();
        services.AddScoped<IAuthTokenService, AuthTokenService>();

        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblies(Assembly.GetExecutingAssembly()));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        return services;
    }
}