using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.ProjectMemberFeatures.Queries;

public class GetAllProjectMembersQueryTest:BaseTest
{
    private IUserRepository _userRepository = null!;
    private IProjectRepository _projectRepository = null!;
    private IProjectMemberRepository _projectMemberRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
        _projectRepository = new ProjectRepository(Context);
        _projectMemberRepository = new ProjectMemberRepository(Context);
    }
    
    [Test]
    public async Task GetAllProjectMembersQuery_ResponseShouldBeSuccess()
    {
        //Arrange
        
        const int testingProjectId = 133113;
        const int testingRequestedUserId = 633341;

        const int testingFirstMemberId = 66653;
        const int testingSecondMemberId = 66654;

        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        List<ProjectMember> projectMembersForTest = Builder<ProjectMember>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingFirstMemberId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingSecondMemberId)
            .Build()
            .ToList();
        await _projectMemberRepository.AddRangeAsync(projectMembersForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.OwnerId = testingRequestedUserId)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await _userRepository.AddAsync(userForTest);
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_projectMemberRepository, _projectRepository, _userRepository);
        
        //Act
        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        IEnumerable<ProjectMember> projectMemberListAfterHandler =
            await _projectMemberRepository.GetAllProjectMembersByProjectIdAsync(testingProjectId);

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        response.Result.Count.ShouldBe(projectMemberListAfterHandler.Count());
    }

    [Test]
    public async Task GetAllProjectMembersQuery_RequestedUserIsNotOwnerButIsAdmin_ResponseShouldBeSuccess()
    {
        //Arrange
        
        const int testingProjectId = 133113;
        const int testingRequestedUserId = 633341;

        const int testingFirstMemberId = 66653;
        const int testingSecondMemberId = 66654;

        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        List<ProjectMember> projectMembersForTest = Builder<ProjectMember>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingFirstMemberId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingSecondMemberId)
            .Build()
            .ToList();
        await _projectMemberRepository.AddRangeAsync(projectMembersForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.Admin)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await _userRepository.AddAsync(userForTest);
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_projectMemberRepository, _projectRepository, _userRepository);
        
        //Act
        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        IEnumerable<ProjectMember> projectMemberListAfterHandler =
            await _projectMemberRepository.GetAllProjectMembersByProjectIdAsync(testingProjectId);

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        response.Result.Count.ShouldBe(projectMemberListAfterHandler.Count());
    }

    [Test]
    public async Task GetAllProjectMembersQuery_ProjectNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        
        const int testingProjectId = 33325;
        const int testingRequestedUserId = 66509;
        
        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_projectMemberRepository, _projectRepository, _userRepository);
        
        //Act
        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }

    [Test]
    public async Task GetAllProjectMembersQuery_RequestedUserIsNotOwner_ResponseShouldBeNotSuccess()
    {
        //Arrange
        
        const int testingProjectId = 33325;
        const int testingRequestedUserId = 66509;
        
        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await _userRepository.AddAsync(userForTest);
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_projectMemberRepository, _projectRepository, _userRepository);
        
        //Act
        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
}