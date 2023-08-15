using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Domain.Common;
using Stockband.Domain.Models;

namespace Stockband.Application.Features.UserFeatures.Queries.RefreshToken;

public class RefreshTokenQueryHandler:IRequestHandler<RefreshTokenQuery, BaseResponse<RefreshTokenQueryViewModel>>
{
    private readonly IAuthTokenService _refreshTokenService;

    public RefreshTokenQueryHandler(IAuthTokenService refreshTokenService)
    {
        _refreshTokenService = refreshTokenService;
    }

    public async Task<BaseResponse<RefreshTokenQueryViewModel>> Handle(RefreshTokenQuery request, CancellationToken cancellationToken)
    {
        BaseResponse<AuthTokenResult> authTokenResult = await _refreshTokenService.RefreshToken(request.RefreshToken);
        if (!authTokenResult.Success)
        {
            return new BaseResponse<RefreshTokenQueryViewModel>(authTokenResult.Errors);
        }

        RefreshTokenQueryViewModel result = new RefreshTokenQueryViewModel
        {
            AccessToken = authTokenResult.Result.AccessToken,
            RefreshToken = authTokenResult.Result.UserRefreshToken.Token
        };
        
        return new BaseResponse<RefreshTokenQueryViewModel>(result);
    }
}