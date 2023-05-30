using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;

public class RemoveMemberFromProjectCommandHandler:IRequestHandler<RemoveMemberFromProjectCommand, BaseResponse>
{
    private readonly IProjectMemberRepository _projectMemberRepository;

    public RemoveMemberFromProjectCommandHandler(
        IProjectMemberRepository projectMemberRepository)
    {
        _projectMemberRepository = projectMemberRepository;
    }
    
    public async Task<BaseResponse> Handle(RemoveMemberFromProjectCommand request, CancellationToken cancellationToken)
    {
        ProjectMember? projectMember = await _projectMemberRepository
            .GetProjectMemberIncludedProjectAndMemberAsync(request.ProjectId, request.MemberId);
        if (projectMember == null)
        {
            return new BaseResponse(new ObjectNotFound(typeof(ProjectMember), request.ProjectId), 
                BaseErrorCode.ProjectMemberNotExists);
        }
        
        if (projectMember.Project.OwnerId != request.ProjectOwnerId)
        {
            return new BaseResponse(new UnauthorizedOperationException(), 
                BaseErrorCode.UserUnauthorizedOperation);
        }
        
        await _projectMemberRepository.DeleteAsync(projectMember);
        return new BaseResponse();
    }
}