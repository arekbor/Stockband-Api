using System.Security.Cryptography;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.FeatureServices;

public class RefreshTokenService:IRefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
    }

    /// <summary>
    /// Generates and stores a new refresh token in the database.
    /// </summary>
    /// <param name="userId">The id of the user associated with the new refresh token.</param>
    /// <param name="ipAddress">The ip of the user for audit purposes.</param>
    /// <param name="refreshTokenDaysExpires">The number of days until the token becomes inactive.</param>
    /// <returns>A generated refresh token.</returns>
    public async Task<BaseResponse<string>> GetRefreshToken
        (int userId, string ipAddress, double refreshTokenDaysExpires)
    {
        string generatedToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        
        //Verifies whether the token is not created on sure
        RefreshToken? token = await _refreshTokenRepository.GetRefreshTokenByToken(generatedToken);
        if (token != null)
        {
            return new BaseResponse<string>(new ObjectAlreadyCreatedException(typeof(RefreshToken)), 
                BaseErrorCode.RefreshTokenAlreadyExists);
        }
        
        RefreshToken refreshToken = new RefreshToken
        {
            Token = generatedToken,
            Expires = DateTime.Now.AddDays(refreshTokenDaysExpires),
            CreatedByIp = ipAddress,
            UserId = userId
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        
        return new BaseResponse<string>(generatedToken);
    }
    
    /// <summary>
    /// Recursively revokes descendant refresh tokens based on the specified parent refresh token.
    /// </summary>
    /// <param name="refreshToken">The parent refresh token whose descendants need to be revoked.</param>
    /// <param name="ipAddress">The ip of the user for audit purposes.</param>
    /// <param name="reasonRevoke">The reason for revoking the tokens.</param>
    public async Task RevokeDescendantRefreshTokens
        (RefreshToken refreshToken, string ipAddress, RefreshTokenReasonRevoke reasonRevoke)
    {
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            RefreshToken? childToken = await _refreshTokenRepository
                .GetRefreshTokenByReplacedByToken(refreshToken.ReplacedByToken);

            if (childToken != null)
            {
                if (childToken.IsActive)
                {
                    await RevokeRefreshToken(childToken, ipAddress, RefreshTokenReasonRevoke.AttemptedReuseRevokedToken);
                }
                else
                {
                    await RevokeDescendantRefreshTokens(childToken, ipAddress, reasonRevoke);
                }
            }
        }
    }
    
    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to be revoked.</param>
    /// <param name="ipAddress">The ip of the user for audit purposes.</param>
    /// <param name="reasonRevoke">The reason for revoking the tokens.</param>
    /// <param name="replacedByToken">An optional refresh token that will replace the revoked token for audit tracking.</param>
    public async Task RevokeRefreshToken
        (RefreshToken refreshToken, string ipAddress, RefreshTokenReasonRevoke reasonRevoke, string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.Now;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoke = reasonRevoke;
        refreshToken.ReplacedByToken = replacedByToken;

        await _refreshTokenRepository.UpdateAsync(refreshToken);
    }

    /// <summary>
    /// Removes old refresh tokens for a specified user.
    /// </summary>
    /// <param name="userId">The id of the user for whom old refresh tokens will be fetched and deleted.</param>
    /// <param name="ttlRefreshTokenDays">Number of days for considering refresh tokens as old.</param>
    public async Task RemoveOldRefreshTokens(int userId, double ttlRefreshTokenDays)
    {
        List<RefreshToken> refreshTokens = await _refreshTokenRepository
            .GetAllUserRefreshTokens(userId);

        refreshTokens = refreshTokens
            .Where(x => !x.IsActive && x.CreatedAt.AddDays(ttlRefreshTokenDays) <= DateTime.Now)
            .ToList();

        await _refreshTokenRepository.DeleteRangeAsync(refreshTokens);
    }
}