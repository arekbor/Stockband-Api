using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Domain.Models;

namespace Stockband.Application.Features.UserFeatures.Queries.LoginUser;

public class LoginUserQueryHandler:IRequestHandler<LoginUserQuery, BaseResponse<LoginUserQueryViewModel>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    private readonly IAuthTokenService _refreshTokenService;

    public LoginUserQueryHandler(
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService, 
        IAuthTokenService refreshTokenService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
        _refreshTokenService = refreshTokenService;
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
            return new BaseResponse<LoginUserQueryViewModel>(new UnauthorizedOperationException(), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
        }
        
        BaseResponse<AuthTokenResult> authTokenResult = await _refreshTokenService.GetTokens(user);
        if (!authTokenResult.Success)
        {
            return new BaseResponse<LoginUserQueryViewModel>(authTokenResult.Errors);
        }

        LoginUserQueryViewModel result = new LoginUserQueryViewModel
        {
            AccessToken = authTokenResult.Result.AccessToken,
            RefreshToken = authTokenResult.Result.UserRefreshToken.Token
        };
        
        return new BaseResponse<LoginUserQueryViewModel>(result);
    }
}