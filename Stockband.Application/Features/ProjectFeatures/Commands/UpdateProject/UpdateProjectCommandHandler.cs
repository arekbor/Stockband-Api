using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommandHandler:IRequestHandler<UpdateProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectFeaturesService _projectFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository,
        IProjectFeaturesService projectFeaturesService,
        IAuthenticationUserService authenticationUserService)
    {
        _projectRepository = projectRepository;
        _projectFeaturesService = projectFeaturesService;
        _authenticationUserService = authenticationUserService;
    }
    
    public async Task<BaseResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(Project), request.ProjectId), 
                BaseErrorCode.ProjectNotExists);
        }

        if (!_authenticationUserService.IsAuthorized(project.OwnerId))
        {
            return new BaseResponse(new UnauthorizedOperationException(),
                BaseErrorCode.UserUnauthorizedOperation);
        }

        if (await _projectFeaturesService.IsProjectNameAlreadyExists(request.ProjectName))
        {
            return new BaseResponse(new ObjectAlreadyCreatedException(typeof(Project), request.ProjectName), 
                BaseErrorCode.ProjectAlreadyCreated);
        }

        project.Name = request.ProjectName;
        project.Description = request.ProjectDescription;

        await _projectRepository.UpdateAsync(project);

        return new BaseResponse();
    }
}