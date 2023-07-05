using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;

public class GetLoggedUserQueryHandler:IRequestHandler<GetLoggedUserQuery, BaseResponse<GetLoggedUserQueryViewModel>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    public GetLoggedUserQueryHandler(
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService)
    {
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
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
            return new BaseResponse<GetLoggedUserQueryViewModel>(new UnauthorizedOperationException(), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
        }
        
        if (!_userFeaturesService.VerifyHashedPassword(request.Password, user.Password))
        {
            return new BaseResponse<GetLoggedUserQueryViewModel>(new UnauthorizedOperationException(), 
                BaseErrorCode.WrongEmailOrPasswordLogin);
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