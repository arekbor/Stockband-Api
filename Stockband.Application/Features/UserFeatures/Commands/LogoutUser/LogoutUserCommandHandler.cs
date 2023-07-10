using MediatR;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain;

namespace Stockband.Application.Features.UserFeatures.Commands.LogoutUser;

public class LogoutUserCommandHandler:IRequestHandler<LogoutUserCommand, BaseResponse>
{
    private readonly IAuthenticationUserService _authenticationUserService;

    public LogoutUserCommandHandler(IAuthenticationUserService authenticationUserService)
    {
        _authenticationUserService = authenticationUserService;
    }
    
    public async Task<BaseResponse> Handle(LogoutUserCommand request, CancellationToken cancellationToken)
    {
        _authenticationUserService.ClearJwtCookie();

        return new BaseResponse();
    }
}