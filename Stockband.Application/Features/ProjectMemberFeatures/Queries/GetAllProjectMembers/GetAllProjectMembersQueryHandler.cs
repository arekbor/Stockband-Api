using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;

namespace Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;

public class GetAllProjectMembersQueryHandler:IRequestHandler<GetAllProjectMembersQuery, BaseResponse<List<GetAllProjectMembersQueryViewModel>>>
{
    private readonly IProjectMemberRepository _projectMemberRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserRepository _userRepository;

    public GetAllProjectMembersQueryHandler(
        IProjectMemberRepository projectMemberRepository, 
        IProjectRepository projectRepository,
        IUserRepository userRepository)
    {
        _projectMemberRepository = projectMemberRepository;
        _projectRepository = projectRepository;
        _userRepository = userRepository;
    }
    public async Task<BaseResponse<List<GetAllProjectMembersQueryViewModel>>>Handle(GetAllProjectMembersQuery request, CancellationToken cancellationToken)
    {
        User? requestedUser = await _userRepository.GetByIdAsync(request.RequestedUserId);
        if (requestedUser == null)
        {
            return new BaseResponse<List<GetAllProjectMembersQueryViewModel>>
            (new ObjectNotFound(typeof(User), request.RequestedUserId), 
                BaseErrorCode.RequestedUserNotExists);
        }
        
        Project? project = await _projectRepository.GetByIdAsync(request.ProjectId);
        if (project == null)
        {
            return new BaseResponse<List<GetAllProjectMembersQueryViewModel>>(
                new ObjectNotFound(typeof(Project), request.ProjectId), 
                BaseErrorCode.ProjectNotExists);
        }
        
        if (!requestedUser.IsAdminOrSameAsUser(project.OwnerId))
        {
            return new BaseResponse<List<GetAllProjectMembersQueryViewModel>>
            (new UnauthorizedOperationException(), BaseErrorCode.UserUnauthorizedOperation);
        }
        
        return new BaseResponse<List<GetAllProjectMembersQueryViewModel>>
            (await GetAllProjectMembersQueryViewModels(request.ProjectId));
    }

    private async Task<List<GetAllProjectMembersQueryViewModel>> GetAllProjectMembersQueryViewModels(int projectId)
    {
        IEnumerable<ProjectMember> projectMembers = 
            await _projectMemberRepository.GetAllProjectMembersByProjectIdAsync(projectId);

        List<GetAllProjectMembersQueryViewModel> projectMembersQueryViewModels =
            new List<GetAllProjectMembersQueryViewModel>();

        foreach (ProjectMember projectMember in projectMembers)
        {
            GetAllProjectMembersQueryViewModel projectMembersQueryViewModel = new GetAllProjectMembersQueryViewModel
            {
                MemberId = projectMember.MemberId,
                ProjectId = projectMember.ProjectId
            };
            projectMembersQueryViewModels.Add(projectMembersQueryViewModel);
        }

        return projectMembersQueryViewModels;
    }
}