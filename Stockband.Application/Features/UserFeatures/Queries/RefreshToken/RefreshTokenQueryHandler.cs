using MediatR;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Queries.RefreshToken;

public class RefreshTokenQueryHandler:IRequestHandler<RefreshTokenQuery, BaseResponse<RefreshTokenQueryViewModel>>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IAuthenticationUserService _authenticationUserService;
    private readonly IConfigurationHelperService _configurationHelperService;
    private readonly IUserFeaturesService _userFeaturesService;
    public RefreshTokenQueryHandler(IRefreshTokenRepository refreshTokenRepository, 
        IRefreshTokenService refreshTokenService, 
        IAuthenticationUserService authenticationUserService, 
        IConfigurationHelperService configurationHelperService, 
        IUserFeaturesService userFeaturesService)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenService = refreshTokenService;
        _authenticationUserService = authenticationUserService;
        _configurationHelperService = configurationHelperService;
        _userFeaturesService = userFeaturesService;
    }
    
    public async Task<BaseResponse<RefreshTokenQueryViewModel>> Handle
        (RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        BaseResponse<string> tokenResponse = _userFeaturesService
            .GetRefreshTokenFromSource(request.Token, request.Cookie);
        if (!tokenResponse.Success)
        {
            return new BaseResponse<RefreshTokenQueryViewModel>(tokenResponse.Errors);
        }

        Domain.Entities.RefreshToken? refreshToken = await _refreshTokenRepository
            .GetRefreshTokenByToken(tokenResponse.Result);

        if (refreshToken == null)
        {
            return new BaseResponse<RefreshTokenQueryViewModel>(
                new ObjectNotFound(typeof(Domain.Entities.RefreshToken)),
                BaseErrorCode.RefreshTokenNotFound);
        }
        string ipAddress = _authenticationUserService.GetUserIp();
        
        if (refreshToken.IsRevoked)
        {
            await _refreshTokenService.RevokeDescendantRefreshTokens
                (refreshToken, ipAddress, RefreshTokenReasonRevoke.AttemptedReuseRevokedToken);
        }

        if (!refreshToken.IsActive)
        {
            return new BaseResponse<RefreshTokenQueryViewModel>(new PerformRestrictedOperationException(), 
                BaseErrorCode.InvalidRefreshToken);
        }
        
        double refreshTokenDaysExpires = _configurationHelperService.GetRefreshTokenExpiresInDays();
        BaseResponse<string> newRefreshToken = await _refreshTokenService
            .GetRefreshToken(refreshToken.UserId, ipAddress, refreshTokenDaysExpires);
        
        await _refreshTokenService
            .RevokeRefreshToken(refreshToken, 
                ipAddress, 
                RefreshTokenReasonRevoke.ReplacedByNewToken, 
                newRefreshToken.Result);

        double refreshTokenTtLInDays = _configurationHelperService.GetRefreshTokenTtLInDays();
        await _refreshTokenService.RemoveOldRefreshTokens(refreshToken.UserId, refreshTokenTtLInDays);

        User user = refreshToken.User;

        string newAccessToken = _authenticationUserService
            .GetAccessToken(user.Id, user.Username, user.Email, user.Role);
        
        AuthorizationTokensResponse tokensResponse = _userFeaturesService
            .ComposeAuthorizationTokens(newAccessToken, newRefreshToken.Result, request.Cookie);

        RefreshTokenQueryViewModel response = new RefreshTokenQueryViewModel
        {
            TokensResponse = tokensResponse
        };
        
        return new BaseResponse<RefreshTokenQueryViewModel>(response);
    }
}