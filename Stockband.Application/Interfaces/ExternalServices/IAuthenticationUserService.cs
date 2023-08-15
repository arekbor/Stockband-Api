using Stockband.Domain.Enums;

namespace Stockband.Application.Interfaces.ExternalServices;

public interface IAuthenticationUserService
{
    string GetAccessToken(int userId, string username, string email, UserRole role);
    string GetRefreshToken();
    string GetUserIp();
    int GetUserId();
    bool IsAuthorized(int userId);
}