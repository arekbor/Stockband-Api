namespace Stockband.Application.Interfaces.CommonServices;

public interface IConfigurationHelperCommonService
{ 
    int GetProjectsLimitPerUser();
    int GetProjectMembersLimitPerProject();
}