using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler:IRequestHandler<UpdatePasswordCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    public UpdatePasswordCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
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

        bool verify = BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password);
        if (!verify)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserUnauthorizedOperation);
        }

        string hash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        user.Password = hash;

        await _userRepository.UpdateAsync(user);
        return new BaseResponse();
    }
}