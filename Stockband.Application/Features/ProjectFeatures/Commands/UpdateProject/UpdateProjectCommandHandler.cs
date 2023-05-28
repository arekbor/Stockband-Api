using FluentValidation.Results;
using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;

public class UpdateProjectCommandHandler:IRequestHandler<UpdateProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;

    public UpdateProjectCommandHandler(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }
    
    public async Task<BaseResponse> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        UpdateProjectCommandValidator validator = new UpdateProjectCommandValidator();
        ValidationResult validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new BaseResponse(validationResult);
        }

        Project? project = await _projectRepository.GetProjectByIdWithIncludedUserAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(Project), request.ProjectId), 
                BaseErrorCode.ProjectNotExists);
        }

        if (project.OwnerId != request.OwnerId)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserUnauthorizedOperation);
        }

        Project? projectNameVerify = await _projectRepository.GetProjectByNameAsync(request.ProjectName);
        if (projectNameVerify != null)
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