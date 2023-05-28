using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Commands.RegisterUser;

public class RegisterUserCommandHandler:IRequestHandler<RegisterUserCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    public RegisterUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<BaseResponse> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        RegisterUserCommandValidator validator = new RegisterUserCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }
        
        User? userResponse =  await _userRepository.GetUserByEmailAsync(request.Email);

        if (userResponse != null)
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(User)), 
                BaseErrorCode.UserAlreadyCreated);
        }
        
        string hash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        User user = new User
        {
            Username = request.Username,
            Email = request.Email,
            Role = UserRole.User,
            Password = hash
        };

        await _userRepository.AddAsync(user);
        return new BaseResponse();
    }
}