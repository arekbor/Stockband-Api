using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.UserFeatures.Queries.GetUserById;

public class GetUserByIdQueryHandler:IRequestHandler<GetUserByIdQuery, BaseResponse<GetUserByIdQueryViewModel>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<BaseResponse<GetUserByIdQueryViewModel>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        User? user = await _userRepository.GetByIdAsync(request.Id);
        if (user == null)
        {
            return new BaseResponse<GetUserByIdQueryViewModel>(new ObjectNotFound(typeof(User), request.Id), 
                BaseErrorCode.UserNotFound);
        }

        GetUserByIdQueryViewModel userViewModel = new GetUserByIdQueryViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };

        return new BaseResponse<GetUserByIdQueryViewModel>(userViewModel);
    }
}