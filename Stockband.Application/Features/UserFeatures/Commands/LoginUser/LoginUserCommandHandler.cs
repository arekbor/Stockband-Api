using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.LoginUser;

public class LoginUserCommandHandler:IRequestHandler<LoginUserCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;

    public LoginUserCommandHandler(
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService, 
        IAuthenticationUserService authenticationUserService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
        _authenticationUserService = authenticationUserService;
    }

    public async Task<BaseResponse> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
        }
        
        if (!_userFeaturesService.VerifyHashedPassword(request.Password, user.Password))
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
        }
        
        string jwtToken = _authenticationUserService.GenerateJwtToken
        (
            user.Id.ToString(), 
            user.Username, 
            user.Email,
            user.Role.ToString()
        );
        _authenticationUserService.AddJwtCookie(jwtToken);

        return new BaseResponse();
    }
}