using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
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

    public RemoveProjectCommandHandler(
        IProjectRepository projectRepository,
        IUserRepository userRepository,
        IProjectMemberFeaturesService projectMemberFeaturesService)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _projectMemberFeaturesService = projectMemberFeaturesService;
    }
    public async Task<BaseResponse> Handle(RemoveProjectCommand request, CancellationToken cancellationToken)
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