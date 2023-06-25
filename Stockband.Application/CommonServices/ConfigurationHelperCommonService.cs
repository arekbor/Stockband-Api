using Microsoft.Extensions.Configuration;
using Stockband.Application.Interfaces.CommonServices;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.CommonServices;

public class ConfigurationHelperCommonService:IConfigurationHelperCommonService
{
    private readonly IConfiguration _configuration;
    public ConfigurationHelperCommonService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public int GetProjectsLimitPerUser() => 
        Get<int>("ProjectFeaturesSettings:ProjectsLimitPerUser");

    public int GetProjectMembersLimitPerProject() => 
        Get<int>("ProjectMemberFeaturesSettings:ProjectMembersLimitPerProject");

    public string GetJwtKey() => 
        Get<string>("JwtKey");

    public string GetJwtAudience() => 
        Get<string>("JwtAudience");

    public string GetJwtIssuer() => 
        Get<string>("JwtIssuer");
    
    public double GetJwtExpires() => 
        Get<double>("JWTExpires");
    
    public double GetCookieExpires() => 
        Get<double>("CookieExpires");
    
    public string GetCookieName() => 
        Get<string>("CookieName");

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