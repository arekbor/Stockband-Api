using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;

public class CreateProjectCommandHandler:IRequestHandler<CreateProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserFeaturesService _userFeaturesService;
    private readonly IProjectFeaturesService _projectFeaturesService;
    
    public CreateProjectCommandHandler(
        IProjectRepository projectRepository, 
        IUserRepository userRepository, 
        IUserFeaturesService userFeaturesService,
        IProjectFeaturesService projectFeaturesService)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _userFeaturesService = userFeaturesService;
        _projectFeaturesService = projectFeaturesService;
    }
    public async Task<BaseResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        CreateProjectCommandValidator validator = new CreateProjectCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }
        
        if (await _projectFeaturesService.IsProjectsLimitExceeded(request.RequestedUserId))
        {
            return new BaseResponse(
                new PerformRestrictedOperationException(), BaseErrorCode.ProjectsLimitPerUserExceeded);
        }
        
        if (!await _userFeaturesService.IsUserExists(request.RequestedUserId))
        {
            return new BaseResponse(
                new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        if (await _projectFeaturesService.IsProjectNameAlreadyExists(request.ProjectName))
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(Project), request.ProjectName), 
                BaseErrorCode.ProjectAlreadyCreated);
        }

        Project newProject = new Project
        {
            OwnerId = request.RequestedUserId,
            Name = request.ProjectName,
            Description = request.ProjectDescription,
        };

        await _projectRepository.AddAsync(newProject);
        return new BaseResponse();
    }
}