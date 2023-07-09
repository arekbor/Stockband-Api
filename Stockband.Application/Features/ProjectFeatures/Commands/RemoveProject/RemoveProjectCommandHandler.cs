using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;

public class RemoveProjectCommandHandler:IRequestHandler<RemoveProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectMemberFeaturesService _projectMemberFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;

    public RemoveProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IProjectMemberFeaturesService projectMemberFeaturesService,
        IAuthenticationUserService authenticationUserService)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _projectMemberFeaturesService = projectMemberFeaturesService;
        _authenticationUserService = authenticationUserService;
    }
    public async Task<BaseResponse> Handle(RemoveProjectCommand request, CancellationToken cancellationToken)
    {
        int currentUserId = _authenticationUserService.GetCurrentUserId();
        
        User? requestedUser = await _userRepository.GetByIdAsync(currentUserId);
        if (requestedUser == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(User), currentUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(Project), request.ProjectId),
                BaseErrorCode.ProjectNotExists);
        }
        
        if (!requestedUser.IsAdminOrSameAsUser(project.OwnerId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }

        if (await _projectMemberFeaturesService.IsAnyProjectMemberBelongToProject(request.ProjectId))
        {
            return new BaseResponse(new PerformRestrictedOperationException(), 
                BaseErrorCode.UserOperationRestricted);
        }
        
        await _projectRepository.DeleteAsync(project);

        return new BaseResponse();
    }
}