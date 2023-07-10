using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.UpdateRole;

public class UpdateRoleCommandHandler:IRequestHandler<UpdateRoleCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    public UpdateRoleCommandHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<BaseResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        UpdateRoleCommandValidator validator = new UpdateRoleCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }
        
        User? user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.UserId),
                BaseErrorCode.UserNotExists);
        }
        
        user.Role = request.Role;
        await _userRepository.UpdateAsync(user);

        return new BaseResponse();
    }
}