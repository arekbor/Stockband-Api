using Microsoft.Extensions.Configuration;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Exceptions;

namespace Stockband.Infrastructure.Services;

public class ConfigurationHelperService:IConfigurationHelperService
{
    private readonly IConfiguration _configuration;

    public ConfigurationHelperService(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public int GetProjectsLimitPerUser() => 
        Get<int>("ProjectsLimitPerUser");

    public int GetProjectMembersLimitPerProject() => 
        Get<int>("ProjectMembersLimitPerProject");

    public string GetAccessTokenPrivateKey() => 
        Get<string>("AccessTokenPrivateKey");

    public string GetAccessTokenAudience() => 
        Get<string>("AccessTokenAudience");

    public string GetAccessTokenIssuer() => 
        Get<string>("AccessTokenIssuer");
    
    public double GetAccessTokenExpiresInMinutes() => 
        Get<double>("AccessTokenExpiresInMinutes");
    
    private T Get<T>(string key)
    {
        T? value = _configuration.GetValue<T>(key);
        if (value == null)
        {
            throw new ObjectNotFound(typeof(IConfiguration), key);
        }

        return value;
    }
}