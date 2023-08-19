using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;

namespace Stockband.Application.Interfaces.FeatureServices;

public interface IRefreshTokenService
{
    Task<BaseResponse<string>> GetRefreshToken
        (int userId, string ipAddress, double refreshTokenDaysExpires);
    
    Task RevokeRefreshToken
    (RefreshToken refreshToken, string ipAddress, RefreshTokenReasonRevoke reasonRevoke,
        string? replacedByToken = null);

    Task RevokeDescendantRefreshTokens
        (RefreshToken refreshToken, string ipAddress, RefreshTokenReasonRevoke reasonRevoke);

    Task RemoveOldRefreshTokens(int userId, double ttlRefreshTokenDays);
}