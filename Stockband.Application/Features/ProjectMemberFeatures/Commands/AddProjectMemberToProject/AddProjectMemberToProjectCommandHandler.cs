using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
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
    private readonly IUserFeaturesService _userFeaturesService;
    private readonly IProjectMemberFeaturesService _projectMemberFeaturesService;

    public AddProjectMemberToProjectCommandHandler(
        IUserRepository userRepository, 
        IProjectRepository projectRepository, 
        IProjectMemberRepository projectMemberRepository, 
        IUserFeaturesService userFeaturesService,
        IProjectMemberFeaturesService projectMemberFeaturesService)
    {
        _userRepository = userRepository;
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
        _userFeaturesService = userFeaturesService;
        _projectMemberFeaturesService = projectMemberFeaturesService;
    }
    public async Task<BaseResponse> Handle(AddProjectMemberToProjectCommand request, CancellationToken cancellationToken)
    {
        User? requestedUser = await _userRepository.GetByIdAsync(request.RequestedUserId); 
        if (requestedUser == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(Project), request.ProjectId), 
                BaseErrorCode.ProjectNotExists);
        }
        
        if (await _projectMemberFeaturesService.IsProjectMembersLimitExceeded(request.ProjectId))
        {
            return new BaseResponse(
                new PerformRestrictedOperationException(), BaseErrorCode.ProjectMembersLimitPerProjectExceeded);
        }
        
        if (!requestedUser.IsAdminOrSameAsUser(project.OwnerId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }
        
        if (!await _userFeaturesService.IsUserExists(request.MemberId))
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.MemberId), 
                BaseErrorCode.MemberForProjectMemberNotExists);
        }
        
        if (project.OwnerId == request.MemberId)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserOperationRestricted);
        }

        if (await _projectMemberFeaturesService.IsProjectMemberBelongToProject(request.ProjectId, request.MemberId))
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