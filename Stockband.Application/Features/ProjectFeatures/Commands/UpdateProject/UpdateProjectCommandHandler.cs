using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.Interfaces.Services;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommandHandler:IRequestHandler<UpdateProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;
    private readonly IProjectFeaturesService _projectFeaturesService;
    private readonly IAuthenticationUserService _authenticationUserService;

    public UpdateProjectCommandHandler(
        IProjectRepository projectRepository, 
        IUserRepository userRepository,
        IProjectFeaturesService projectFeaturesService,
        IAuthenticationUserService authenticationUserService)
    {
        _projectRepository = projectRepository;
        _userRepository = userRepository;
        _projectFeaturesService = projectFeaturesService;
        _authenticationUserService = authenticationUserService;
    }
    
    public async Task<BaseResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        UpdateProjectCommandValidator validator = new UpdateProjectCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }

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
        
        if (await _projectFeaturesService.IsProjectNameAlreadyExists(request.ProjectName))
        {
            return new BaseResponse(new ObjectIsAlreadyCreatedException(typeof(Project), request.ProjectName), 
                BaseErrorCode.ProjectAlreadyCreated);
        }

        project.Name = request.ProjectName;
        project.Description = request.ProjectDescription;

        await _projectRepository.UpdateAsync(project);

        return new BaseResponse();
    }
}