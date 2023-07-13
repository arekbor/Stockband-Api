using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.RegisterUser;

public class RegisterUserCommandHandler:IRequestHandler<RegisterUserCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    public RegisterUserCommandHandler(
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
    }
    public async Task<BaseResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userFeaturesService.IsEmailAlreadyUsed(request.Email))
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(User), request.Email), 
                BaseErrorCode.UserEmailAlreadyExists);
        }
        
        User user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Role = UserRole.User,
            Password = _userFeaturesService.HashPassword(request.Password)
        };

        await _userRepository.AddAsync(user);
        return new BaseResponse();
    }
}