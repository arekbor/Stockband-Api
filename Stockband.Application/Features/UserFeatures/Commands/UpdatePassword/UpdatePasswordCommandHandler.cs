using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler:IRequestHandler<UpdatePasswordCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    public UpdatePasswordCommandHandler(
        IUserRepository userRepository,
        IUserFeaturesService userFeaturesService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
    }
    public async Task<BaseResponse> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        UpdatePasswordCommandValidator validator = new UpdatePasswordCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }
        
        User? user = await _userRepository.GetByIdAsync(request.RequestedUserId);
        if (user == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        if (!_userFeaturesService.VerifyHashedPassword(request.CurrentPassword, user.Password))
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserUnauthorizedOperation);
        }
        
        user.Password = _userFeaturesService.HashPassword(request.NewPassword);

        await _userRepository.UpdateAsync(user);
        return new BaseResponse();
    }
}