using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.ExternalServices;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;

public class CreateProjectCommandHandler:IRequestHandler<CreateProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectFeaturesService _projectFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;
    
    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        IProjectFeaturesService projectFeaturesService,
        IAuthenticationUserService authenticationUserService)
    {
        _projectRepository = projectRepository;
        _projectFeaturesService = projectFeaturesService;
        _authenticationUserService = authenticationUserService;
    }
    public async Task<BaseResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        int currentUserId = _authenticationUserService.GetUserId();
        
        if (await _projectFeaturesService.IsProjectsLimitExceeded(currentUserId))
        {
            return new BaseResponse(
                new PerformRestrictedOperationException(), BaseErrorCode.ProjectsLimitPerUserExceeded);
        }
        
        if (await _projectFeaturesService.IsProjectNameAlreadyExists(request.ProjectName))
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(Project), request.ProjectName), 
                BaseErrorCode.ProjectAlreadyCreated);
        }

        Project newProject = new Project
        {
            OwnerId = currentUserId,
            Name = request.ProjectName,
            Description = request.ProjectDescription,
        };

        await _projectRepository.AddAsync(newProject);
        return new BaseResponse();
    }
}