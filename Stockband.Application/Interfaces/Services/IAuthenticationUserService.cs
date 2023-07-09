namespace Stockband.Application.Interfaces.Services;

public interface IAuthenticationUserService
{
    string GenerateJwtToken(string userId, string username, string email, string role);
    void AddJwtCookie(string jwtToken);
    void ClearJwtCookie();
    int GetCurrentUserId();
    IEnumerable<string> GetCurrentUserRoles();
}