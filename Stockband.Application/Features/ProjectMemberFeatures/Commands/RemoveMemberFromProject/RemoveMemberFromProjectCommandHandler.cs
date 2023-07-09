using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;

public class RemoveMemberFromProjectCommandHandler:IRequestHandler<RemoveMemberFromProjectCommand, BaseResponse>
{
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserRepository _userRepository;
    private readonly IAuthenticationUserService _authenticationUserService;

    public RemoveMemberFromProjectCommandHandler(
        IProjectMemberRepository projectMemberRepository,
        IUserRepository userRepository, 
        IAuthenticationUserService authenticationUserService)
    {
        _projectMemberRepository = projectMemberRepository;
        _userRepository = userRepository;
        _authenticationUserService = authenticationUserService;
    }
    
    public async Task<BaseResponse> Handle(RemoveMemberFromProjectCommand request, CancellationToken cancellationToken)
    {
        int currentUserId = _authenticationUserService.GetCurrentUserId();
        
        User? requestedUser = await _userRepository.GetByIdAsync(currentUserId);
        if (requestedUser == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), currentUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        ProjectMember? projectMember = await _projectMemberRepository
            .GetProjectMemberIncludedProjectAndMemberAsync(request.ProjectId, request.MemberId);
        if (projectMember == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(ProjectMember), request.ProjectId, request.MemberId), 
                BaseErrorCode.ProjectMemberNotExists);
        }
        
        if (!requestedUser.IsAdminOrSameAsUser(projectMember.Project.OwnerId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }
        
        await _projectMemberRepository.DeleteAsync(projectMember);
        return new BaseResponse();
    }
}