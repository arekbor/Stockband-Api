namespace Stockband.Application.Interfaces.ExternalServices;

public interface IConfigurationHelperService
{ 
    int GetProjectsLimitPerUser();
    int GetProjectMembersLimitPerProject();
    string GetAccessTokenPrivateKey();
    string GetAccessTokenAudience();
    string GetAccessTokenIssuer();
    double GetAccessTokenExpiresInMinutes();
    double GetRefreshTokenExpiresInDays();
    double GetRefreshTokenTtLInDays();
}