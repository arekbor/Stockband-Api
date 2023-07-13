namespace Stockband.Application.Interfaces.Services;

public interface IConfigurationHelperService
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