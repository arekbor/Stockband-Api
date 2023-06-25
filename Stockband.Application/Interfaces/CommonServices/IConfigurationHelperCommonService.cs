namespace Stockband.Application.Interfaces.CommonServices;

public interface IConfigurationHelperCommonService
{ 
    int GetProjectsLimitPerUser();
    int GetProjectMembersLimitPerProject();
    string GetJwtKey();
    string GetJwtAudience();
    string GetJwtIssuer();
    double GetJwtExpires();
    double GetCookieExpires();
    string GetCookieName();
}