using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Queries.LoginUser;

public class LoginUserQueryHandler:IRequestHandler<LoginUserQuery, BaseResponse<LoginUserQueryViewModel>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IConfigurationHelperService _configurationHelperService;

    public LoginUserQueryHandler(
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService, 
        IAuthenticationUserService authenticationUserService, 
        IRefreshTokenService refreshTokenService, 
        IConfigurationHelperService configurationHelperService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
        _authenticationUserService = authenticationUserService;
        _refreshTokenService = refreshTokenService;
        _configurationHelperService = configurationHelperService;
    }
    
    public async Task<BaseResponse<LoginUserQueryViewModel>> Handle(LoginUserQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return new BaseResponse<LoginUserQueryViewModel>(new UnauthorizedOperationException(), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
        }
        
        if (!_userFeaturesService.VerifyHashedPassword(request.Password, user.Password))
        {
            return new BaseResponse<LoginUserQueryViewModel>(
                new UnauthorizedOperationException("Wrong email or password"), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
        }
        
        string jwtToken = _authenticationUserService.GetAccessToken
        (
            user.Id, 
            user.Username, 
            user.Email,
            user.Role
        );

        string userIp = _authenticationUserService.GetUserIp();
        double refreshTokenDaysExpires = _configurationHelperService.GetRefreshTokenExpiresInDays();
        
        BaseResponse<string> refreshToken = await _refreshTokenService.GetRefreshToken
            (user.Id, userIp, refreshTokenDaysExpires);

        double refreshTokenTtLInDays = _configurationHelperService.GetRefreshTokenTtLInDays();
        await _refreshTokenService.RemoveOldRefreshTokens(user.Id, refreshTokenTtLInDays);

        AuthorizationTokensResponse tokensResponse = _userFeaturesService
            .ComposeAuthorizationTokens(jwtToken, refreshToken.Result, request.Cookie);

        LoginUserQueryViewModel response = new LoginUserQueryViewModel
        {
            TokensResponse = tokensResponse
        };
        
        return new BaseResponse<LoginUserQueryViewModel>(response);
    }
}