using MediatR;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;

namespace Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;

public class GetAllProjectMembersQueryHandler:IRequestHandler<GetAllProjectMembersQuery, BaseResponse<List<GetAllProjectMembersQueryViewModel>>>
{
    private readonly IProjectMemberRepository _projectMemberRepository;

    public GetAllProjectMembersQueryHandler(
        IProjectMemberRepository projectMemberRepository)
    {
        _projectMemberRepository = projectMemberRepository;
    }
    public async Task<BaseResponse<List<GetAllProjectMembersQueryViewModel>>>Handle(GetAllProjectMembersQuery request, CancellationToken cancellationToken)
    {
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