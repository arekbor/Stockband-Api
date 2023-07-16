namespace Stockband.Application.Interfaces.Services;

public interface IAuthenticationUserService
{
    string GetJwtToken(string userId, string username, string email, string role);
    int GetUserId();
    IEnumerable<string> GetRoles();
    bool IsAuthorized(int userId);
}