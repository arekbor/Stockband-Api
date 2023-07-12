namespace Stockband.Api;

internal static class CorsCollection
{
    internal static IServiceCollection AddCorsCollection(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("ReactClientPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
        return services;
    }
}