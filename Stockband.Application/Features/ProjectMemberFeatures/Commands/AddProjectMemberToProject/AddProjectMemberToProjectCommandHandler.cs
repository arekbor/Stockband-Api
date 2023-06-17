using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;

public class AddProjectMemberToProjectCommandHandler:IRequestHandler<AddProjectMemberToProjectCommand, BaseResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;

    public AddProjectMemberToProjectCommandHandler(
        IUserRepository userRepository, 
        IProjectRepository projectRepository, 
        IProjectMemberRepository projectMemberRepository)
    {
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
    }
    public async Task<BaseResponse> Handle(AddProjectMemberToProjectCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(Project), request.ProjectId), 
                BaseErrorCode.ProjectNotExists);
        }

        User? requestedUser = await _userRepository.GetByIdAsync(request.RequestedUserId);
        if (requestedUser == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.UserNotExists);
        }

        if (!requestedUser.IsEntityAccessibleByUser(project.OwnerId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }

        if (project.OwnerId == request.MemberId)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserOperationRestricted);
        }

        User? member = await _userRepository.GetByIdAsync(request.MemberId);
        if (member == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.MemberId), 
                BaseErrorCode.MemberForProjectMemberNotExists);
        }

        IEnumerable<ProjectMember> projectMembers = await _projectMemberRepository
            .GetAllProjectMembersByProjectIdAsync(request.ProjectId);

        bool isMemberAlreadyInProject = projectMembers.Any(x => x.MemberId == request.MemberId);
        if (isMemberAlreadyInProject)
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(ProjectMember)), 
                BaseErrorCode.ProjectMemberAlreadyCreated);
        }

        ProjectMember projectMember = new ProjectMember
        {
            MemberId = request.MemberId,
            ProjectId = request.ProjectId
        };
        
        await _projectMemberRepository.AddAsync(projectMember);
        return new BaseResponse();
    }
}