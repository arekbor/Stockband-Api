using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Domain.Common;

namespace Stockband.Application.Features.UserFeatures.Commands.RevokeToken;

public class RevokeTokenCommandHandler:IRequestHandler<RevokeTokenCommand, BaseResponse>
{
    private readonly IAuthTokenService _authTokenService;

    public RevokeTokenCommandHandler(IAuthTokenService authTokenService)
    {
        _authTokenService = authTokenService;
    }

    public async Task<BaseResponse> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        BaseResponse response =  await _authTokenService.RevokeToken(request.RefreshToken);
        if (!response.Success)
        {
            return new BaseResponse(response.Errors);
        }
        return new BaseResponse();
    }
}