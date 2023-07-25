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

    public LoginUserQueryHandler(
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService, 
        IAuthenticationUserService authenticationUserService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
        _authenticationUserService = authenticationUserService;
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
        
        string jwtToken = _authenticationUserService.GetJwtToken
        (
            user.Id.ToString(), 
            user.Username, 
            user.Email,
            user.Role.ToString()
        );

        LoginUserQueryViewModel result = new LoginUserQueryViewModel
        {
            Token = jwtToken
        };
        
        return new BaseResponse<LoginUserQueryViewModel>(result);
    }
}