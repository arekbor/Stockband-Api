using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;
using Stockband.Domain.Models;

namespace Stockband.Application.FeatureServices;

public class AuthTokenService:IAuthTokenService
{
    private readonly IAuthenticationUserService _authenticationUserService;
    private readonly IUserRepository _userRepository;
    private readonly IConfigurationHelperService _configurationHelperService;

    public AuthTokenService(
        IAuthenticationUserService authenticationUserService, 
        IUserRepository userRepository, 
        IConfigurationHelperService configurationHelperService)
    {
        _authenticationUserService = authenticationUserService;
        _userRepository = userRepository;
        _configurationHelperService = configurationHelperService;
    }
    
    public async Task<BaseResponse<AuthTokenResult>> GetTokens(User user)
    {
        string accessToken = _authenticationUserService.GetAccessToken(user.Id, user.Username, user.Email, user.Role);
        
        string ipAddress = _authenticationUserService.GetUserIp();
        UserRefreshToken refreshToken = await GenerateRefreshToken(ipAddress);

        user.UserRefreshTokens.Add(refreshToken);
        RemoveOldRefreshTokens(user);
        await _userRepository.UpdateAsync(user);

        AuthTokenResult result = new AuthTokenResult
        {
            AccessToken = accessToken,
            UserRefreshToken = refreshToken
        };
        
        return new BaseResponse<AuthTokenResult>(result);
    }
    
    public async Task<BaseResponse<AuthTokenResult>> RefreshToken(string token)
    {
        User? user = await _userRepository.GetUserByRefreshToken(token);
        if (user == null)
        {
            return new BaseResponse<AuthTokenResult>(new ObjectNotFound(typeof(User)), 
                BaseErrorCode.UserNotExists);
        }
        
        string ipAddress = _authenticationUserService.GetUserIp();

        UserRefreshToken refreshToken = user.UserRefreshTokens.Single(x => x.Token == token);
        if (refreshToken.IsRevoked)
        {
            RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, RefreshTokenReasonRevoke.AttemptedReuseRevokedToken);
            await _userRepository.UpdateAsync(user);
        }

        if (!refreshToken.IsActive)
        {
            return new BaseResponse<AuthTokenResult>(new UnauthorizedOperationException("Invalid token"), 
                BaseErrorCode.InvalidRefreshToken);
        }

        UserRefreshToken newRefreshToken = await RotateRefreshToken(refreshToken, ipAddress);
        user.UserRefreshTokens.Add(newRefreshToken);
        RemoveOldRefreshTokens(user);
        await _userRepository.UpdateAsync(user);

        string newAccessToken = _authenticationUserService.GetAccessToken(user.Id, user.Username, user.Email, user.Role);

        
        AuthTokenResult result = new AuthTokenResult
        {
            AccessToken = newAccessToken,
            UserRefreshToken = newRefreshToken
        };
        
        return new BaseResponse<AuthTokenResult>(result);
    }

    public async Task<BaseResponse> RevokeToken(string token)
    {
        User? user = await _userRepository.GetUserByRefreshToken(token);
        if (user == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User)), BaseErrorCode.UserNotExists);
        }

        UserRefreshToken userRefreshToken = user.UserRefreshTokens.Single(x => x.Token == token);
        if (!userRefreshToken.IsActive)
        {
            return new BaseResponse(new UnauthorizedOperationException("Invalid token"),
                BaseErrorCode.InvalidRefreshToken);
        }
        string ipAddress = _authenticationUserService.GetUserIp();
        RevokeRefreshToken(userRefreshToken, ipAddress, RefreshTokenReasonRevoke.ManualRevoked);
        await _userRepository.UpdateAsync(user);

        return new BaseResponse();
    }


    /// <summary>
    /// Revoke all descendant tokens in case this token has been compromised
    /// </summary>
    private void RevokeDescendantRefreshTokens(UserRefreshToken refreshToken, User user, string ipAddress, RefreshTokenReasonRevoke reasonRevoke)
    {
        if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
        {
            UserRefreshToken? childToken =
                user.UserRefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);

            if (childToken == null)
                return;
            
            if (childToken.IsActive)
                RevokeRefreshToken(childToken, ipAddress, reasonRevoke);
            else
                RevokeDescendantRefreshTokens(childToken, user, ipAddress, reasonRevoke);
        }
    }

    private void RemoveOldRefreshTokens(User user)
    {
        double daysRefreshTokenTtl = _configurationHelperService.GetRefreshTokenTTLInDays();
        user.UserRefreshTokens
            .RemoveAll(x => !x.IsActive && x.CreatedAt.AddDays(daysRefreshTokenTtl) <= DateTime.Now);
    }

    private void RevokeRefreshToken
        (UserRefreshToken refreshToken, string ipAddress, RefreshTokenReasonRevoke reasonRevoke ,string? replacedByToken = null)
    {
        refreshToken.Revoked = DateTime.Now;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoke = reasonRevoke;
        refreshToken.ReplacedByToken = replacedByToken;
    }
    
    private async Task<UserRefreshToken> RotateRefreshToken(UserRefreshToken refreshToken, string ipAddress)
    {
        UserRefreshToken newRefreshToken = await GenerateRefreshToken(ipAddress);
        RevokeRefreshToken
            (refreshToken, ipAddress, RefreshTokenReasonRevoke.ReplacedByNewToken, newRefreshToken.Token);
        return newRefreshToken;
    }

    private async Task<UserRefreshToken> GenerateRefreshToken(string ipAddress)
    {
        double daysExpire = _configurationHelperService.GetRefreshTokenExpiresInDays();

        string uniqueToken = _authenticationUserService.GetRefreshToken();
        if (await _userRepository.IsUserRefreshTokenIsUnique(uniqueToken))
        {
            throw new ObjectIsAlreadyCreatedException(typeof(UserRefreshToken), uniqueToken);
        }

        UserRefreshToken refreshToken = new UserRefreshToken
        {
            Token = uniqueToken,
            Expires = DateTime.Now.AddDays(daysExpire),
            CreatedByIp = ipAddress
        };
        return refreshToken;
    }
}