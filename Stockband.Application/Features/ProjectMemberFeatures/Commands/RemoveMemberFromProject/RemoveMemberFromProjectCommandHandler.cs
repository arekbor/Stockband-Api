using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;

public class RemoveMemberFromProjectCommandHandler:IRequestHandler<RemoveMemberFromProjectCommand, BaseResponse>
{
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserRepository _userRepository;

    public RemoveMemberFromProjectCommandHandler(
        IProjectMemberRepository projectMemberRepository,
        IUserRepository userRepository)
    {
        _projectMemberRepository = projectMemberRepository;
        _userRepository = userRepository;
    }
    
    public async Task<BaseResponse> Handle(RemoveMemberFromProjectCommand request, CancellationToken cancellationToken)
    {
        ProjectMember? projectMember = await _projectMemberRepository
            .GetProjectMemberIncludedProjectAndMemberAsync(request.ProjectId, request.MemberId);
        if (projectMember == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(ProjectMember), request.ProjectId, request.MemberId), 
                BaseErrorCode.ProjectMemberNotExists);
        }
        
        User? requestedUser = await _userRepository.GetByIdAsync(request.RequestedUserId);
        if (requestedUser == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.RequestedUserNotExists);
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