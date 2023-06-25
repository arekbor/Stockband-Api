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
    
    public int GetProjectsLimitPerUser()
    {
        return Get<int>("ProjectFeaturesSettings:ProjectsLimitPerUser");
    }

    public int GetProjectMembersLimitPerProject()
    {
        return Get<int>("ProjectMemberFeaturesSettings:ProjectMembersLimitPerProject");
    }

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