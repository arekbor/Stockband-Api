using FizzWare.NBuilder;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Infrastructure;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Builders;

internal class ProjectMemberBuilder:BaseTest
{
    private readonly IProjectMemberRepository _projectMemberRepository;

    public ProjectMemberBuilder(StockbandDbContext context)
    {
        _projectMemberRepository = new ProjectMemberRepository(context);
    }

    internal async Task Build
        (int projectMemberId, int? projectId, int? memberId)
    {
        ProjectMember projectMember = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = projectMemberId)
            .Build();

        if (projectId != null)
        {
            projectMember.ProjectId = (int)projectId;
        }

        if (memberId != null)
        {
            projectMember.MemberId = (int)memberId;
        }

        await _projectMemberRepository.AddAsync(projectMember);
    }

    internal async Task AttachManyMembersToProjectId(int projectMemberId, int projectId, int[] memberIds)
    {
        List<ProjectMember> projectMembers = Builder<ProjectMember>
            .CreateListOfSize(memberIds.Length)
            .All()
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = projectId)
            .Build()
            .ToList();

        int index = 0;
        foreach (ProjectMember projectMember in projectMembers)
        {
            projectMember.MemberId = memberIds[index++];
        }

        await _projectMemberRepository
            .AddRangeAsync(projectMembers);
    }
}