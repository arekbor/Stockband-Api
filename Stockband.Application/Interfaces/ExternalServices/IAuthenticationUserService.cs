namespace Stockband.Application.Interfaces.ExternalServices;

public interface IAuthenticationUserService
{
    string GetAccessToken(string userId, string username, string email, string role);
    int GetUserId();
    IEnumerable<string> GetRoles();
    bool IsAuthorized(int userId);
}