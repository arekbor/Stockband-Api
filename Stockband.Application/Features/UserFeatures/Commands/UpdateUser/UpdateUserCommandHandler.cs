using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateUser;

public class UpdateUserCommandHandler:IRequestHandler<UpdateUserCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUserFeaturesService userFeaturesService
        )
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
    }
    public async Task<BaseResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        UpdateUserCommandValidator validator = new UpdateUserCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }
        
        User? requestedUser = await _userRepository.GetByIdAsync(request.RequestedUserId);
        if (requestedUser == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        if (!requestedUser.IsAdminOrSameAsUser(request.UserId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }
        
        User? user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.UserId), 
                BaseErrorCode.UserNotExists);
        }

        if (await _userFeaturesService.IsEmailAlreadyUsed(request.Email))
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(User), request.Email), 
                BaseErrorCode.UserEmailAlreadyExists);
        }

        user.Email = request.Email;
        user.Username = request.Username;

        await _userRepository.UpdateAsync(user);

        return new BaseResponse();
    }
}