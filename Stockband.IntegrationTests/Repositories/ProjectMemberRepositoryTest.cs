using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.IntegrationTests.Repositories;

[TestFixture]
public class ProjectMemberRepositoryTest:BaseTest
{
    private IProjectMemberRepository _projectMemberRepository = null!;
    private IProjectRepository _projectRepository = null!;
    private IUserRepository _userRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _projectMemberRepository = new ProjectMemberRepository(Context);
        _projectRepository = new ProjectRepository(Context);
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public async Task GetAllProjectMembersByProjectIdAsync_ShouldReturnAllProjectMembers()
    {
        //Arrange
        const int testingProjectId = 54446;
        const int amount = 12;

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        List<ProjectMember> projectMembersForTest = Builder<ProjectMember>
            .CreateListOfSize(200)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(amount)
            .With(x => x.ProjectId = testingProjectId)
            .Build()
            .ToList();
        await _projectMemberRepository.AddRangeAsync(projectMembersForTest);

        //Act
        IEnumerable<ProjectMember> result =
            await _projectMemberRepository.GetAllProjectMembersByProjectIdAsync(testingProjectId);

        //Assert
        result.Count().ShouldBe(amount);
    }

    [Test]
    public async Task GetProjectMemberIncludedProjectAndMemberAsync_ShouldReturnObjectWithIncludes()
    {
        //Arrange
        const int testingProjectId = 1234443;
        const int testingMemberId = 111233;

        User memberForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingMemberId)
            .Build();
        await _userRepository.AddAsync(memberForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        ProjectMember projectMemberForTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingMemberId)
            .Build();
        await _projectMemberRepository.AddAsync(projectMemberForTest);

        //Act
        ProjectMember? projectMember = await _projectMemberRepository
            .GetProjectMemberIncludedProjectAndMemberAsync(testingProjectId, testingMemberId);
        if (projectMember == null)
        {
            throw new ObjectNotFound(typeof(ProjectMember), testingProjectId, testingMemberId);
        }
        
        //Assert
        projectMember.Project.ShouldNotBeNull();
        projectMember.Member.ShouldNotBeNull();
    }
}