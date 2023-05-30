using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;

public class RemoveProjectCommandHandler:IRequestHandler<RemoveProjectCommand, BaseResponse>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IProjectMemberRepository _projectMemberRepository;

    public RemoveProjectCommandHandler(IProjectRepository projectRepository, IProjectMemberRepository projectMemberRepository)
    {
        _projectRepository = projectRepository;
        _projectMemberRepository = projectMemberRepository;
    }
    public async Task<BaseResponse> Handle(RemoveProjectCommand request, CancellationToken cancellationToken)
    {
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
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

        IEnumerable<ProjectMember> projectMembers = await _projectMemberRepository
            .GetAllProjectMembersByProjectIdAsync(request.ProjectId);

        if (projectMembers.Any())
        {
            return new BaseResponse(new PerformRestrictedOperationException(), 
                BaseErrorCode.UserOperationRestricted);
        }

        await _projectRepository.DeleteAsync(project);

        return new BaseResponse();
    }
}