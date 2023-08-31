using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;

public class AddProjectMemberToProjectCommandHandler:IRequestHandler<AddProjectMemberToProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    private readonly IProjectMemberFeaturesService _projectMemberFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;

    public AddProjectMemberToProjectCommandHandler(
        IProjectRepository projectRepository, 
        IProjectMemberRepository projectMemberRepository, 
        IUserFeaturesService userFeaturesService,
        IProjectMemberFeaturesService projectMemberFeaturesService,
        IAuthenticationUserService authenticationUserService)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
        _userFeaturesService = userFeaturesService;
        _projectMemberFeaturesService = projectMemberFeaturesService;
        _authenticationUserService = authenticationUserService;
    }
    public async Task<BaseResponse> Handle(AddProjectMemberToProjectCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(Project), request.ProjectId), 
                BaseErrorCode.ProjectNotFound);
        }
        
        if (await _projectMemberFeaturesService.IsProjectMembersLimitExceeded(request.ProjectId))
        {
            return new BaseResponse(
                new PerformRestrictedOperationException(), BaseErrorCode.ProjectMembersLimitPerProjectExceeded);
        }

        if (!_authenticationUserService.IsAuthorized(project.OwnerId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }

        if (!await _userFeaturesService.IsUserExists(request.MemberId))
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), request.MemberId), 
                BaseErrorCode.MemberForProjectMemberNotFound);
        }
        
        if (project.OwnerId == request.MemberId)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserOperationRestricted);
        }

        if (await _projectMemberFeaturesService.IsProjectMemberBelongToProject(request.ProjectId, request.MemberId))
        {
            return new BaseResponse(new ObjectAlreadyCreatedException(typeof(ProjectMember)), 
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