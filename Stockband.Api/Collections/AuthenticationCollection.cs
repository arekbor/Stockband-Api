using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Stockband.Application.Interfaces.Services;

namespace Stockband.Api;

internal static class AuthenticationCollection
{
    internal static IServiceCollection AddAuthenticationCollection(
        this IServiceCollection services, 
        IConfigurationHelperService configurationHelperService)
    {
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
                
                ValidIssuer = configurationHelperService.GetJwtIssuer(),
                ValidAudience = configurationHelperService.GetJwtAudience(),
                IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(configurationHelperService.GetJwtKey())),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
            };
        });
        return services;
    }
}