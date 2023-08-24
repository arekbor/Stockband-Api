using Stockband.Domain.Common;

namespace Stockband.Application.Interfaces.FeatureServices;

public interface IUserFeaturesService
{
    Task<bool> IsEmailAlreadyUsed(string email);
    string HashPassword(string password);
    bool VerifyHashedPassword(string hashedPassword, string password);
    Task<bool> IsUserExists(int userId);
    AuthorizationTokensResponse ComposeAuthorizationTokens(string accessToken, string refreshToken, bool cookie);
    BaseResponse<string> GetRefreshTokenFromSource(string refreshToken, bool cookie);
}