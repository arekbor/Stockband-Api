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
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configurationHelperService.GetJwtKey())),
                ValidateIssuer = true,
                ValidIssuer = configurationHelperService.GetJwtIssuer(),
                ValidateAudience = true,
                ValidAudience = configurationHelperService.GetJwtAudience(),
            };
            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = ctx =>
                {
                    ctx.Token = ctx.Request.Cookies[configurationHelperService.GetCookieName()];
                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = ctx =>
                {
                    Console.WriteLine(ctx.Exception.ToString());
                    return Task.CompletedTask;
                }
            };
        });
        return services;
    }
}