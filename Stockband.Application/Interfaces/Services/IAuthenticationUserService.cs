using Stockband.Domain.Common;

namespace Stockband.Application.Interfaces.Services;

public interface IAuthenticationUserService
{
    string GenerateJwtToken(string userId, string username, string email, string role);
    void AddJwtCookie(string jwtToken);
    void ClearJwtCookie();
    int GetUserId();
    IEnumerable<string> GetRoles();
    bool IsAuthorized(int userId);
}