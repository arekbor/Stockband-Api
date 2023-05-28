using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;

public class GetLoggedUserQueryHandler:IRequestHandler<GetLoggedUserQuery, BaseResponse<GetLoggedUserQueryViewModel>>
{
    private readonly IUserRepository _userRepository;
    public GetLoggedUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<BaseResponse<GetLoggedUserQueryViewModel>> Handle(GetLoggedUserQuery request, CancellationToken cancellationToken)
    {
        GetLoggedUserQueryValidator validator = new GetLoggedUserQueryValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse<GetLoggedUserQueryViewModel>(validationResult);
        }
        
        User? user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
            return new BaseResponse<GetLoggedUserQueryViewModel>(new UnauthorizedOperationException());
        }
        
        bool verify = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
        if (!verify)
        {
            return new BaseResponse<GetLoggedUserQueryViewModel>(new UnauthorizedOperationException());
        }

        GetLoggedUserQueryViewModel viewModel = new GetLoggedUserQueryViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };

        return new BaseResponse<GetLoggedUserQueryViewModel>(viewModel);
    }
}