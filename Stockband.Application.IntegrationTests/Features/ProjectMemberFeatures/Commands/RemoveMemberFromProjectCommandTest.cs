using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.ProjectMemberFeatures.Commands;

public class RemoveMemberFromProjectCommandTest:BaseTest
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
    public async Task RemoveMemberFromProjectCommand_ResponseShouldBeSuccess()
    {
        //Arrange
        const int testingRequestedUserId = 93342;
        const int testingProjectId = 19653;
        const int testingMemberId = 3003123;
        
        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };

        List<User> membersForTest = Builder<User>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingMemberId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build()
            .ToList();
        await _userRepository.AddRangeAsync(membersForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.OwnerId = testingRequestedUserId)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        ProjectMember projectMemberForTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.MemberId = testingMemberId)
            .With(x => x.ProjectId = testingProjectId)
            .Build();
        await _projectMemberRepository.AddAsync(projectMemberForTest);

        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
            (_projectMemberRepository, _userRepository);
        
        //Act
        IEnumerable<ProjectMember> listOfProjectMemberBeforeHandler = await _projectMemberRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<ProjectMember> listOfProjectMemberAfterHandler = await _projectMemberRepository.GetAllAsync();

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        listOfProjectMemberAfterHandler.Count().ShouldBe(listOfProjectMemberBeforeHandler.Count()-1);
    }

    [Test]
    public async Task RemoveMemberFromProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ResponseShouldBeSuccess()
    {
        //Arrange
        const int testingRequestedUserId = 93342;
        const int testingProjectId = 19653;
        const int testingMemberId = 3003123;
        
        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };

        List<User> usersForTest = Builder<User>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingMemberId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.Admin)
            .With(x => x.Id = testingRequestedUserId)
            .Build()
            .ToList();
        await _userRepository.AddRangeAsync(usersForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        ProjectMember projectMemberForTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.MemberId = testingMemberId)
            .With(x => x.ProjectId = testingProjectId)
            .Build();
        await _projectMemberRepository.AddAsync(projectMemberForTest);

        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
            (_projectMemberRepository, _userRepository);
        
        //Act
        IEnumerable<ProjectMember> listOfProjectMemberBeforeHandler = await _projectMemberRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<ProjectMember> listOfProjectMemberAfterHandler = await _projectMemberRepository.GetAllAsync();

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        listOfProjectMemberAfterHandler.Count().ShouldBe(listOfProjectMemberBeforeHandler.Count()-1);
    }

    [Test]
    public async Task RemoveMemberFromProjectCommand_RequestedUserIsNotOwner_ResponseShouldBeNotSuccess()
    {
        //Arrange
        const int testingRequestedUserId = 2541;
        const int testingProjectId = 1996;
        const int testingMemberId = 100321;
        
        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await _projectRepository.AddAsync(projectForTest);

        List<User> usersForTest = Builder<User>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingMemberId)
            .Build()
            .ToList();
        await _userRepository.AddRangeAsync(usersForTest);

        ProjectMember projectMemberForTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingMemberId)
            .Build();
        await _projectMemberRepository.AddAsync(projectMemberForTest);

        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
            (_projectMemberRepository, _userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }

    [Test]
    public async Task RemoveMemberFromProjectCommand_ProjectMemberNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange    
        const int testingRequestedUserId = 2541;
        const int testingProjectId = 1996;
        const int testingMemberId = 100321;
        
        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
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
            .With(x => x.Id = testingRequestedUserId)
            .With(x => x.Role = UserRole.User)
            .Build();
        await _userRepository.AddAsync(userForTest);

        ProjectMember projectMemberForTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .With(x => x.MemberId = testingMemberId)
            .Build();
        await _projectMemberRepository.AddAsync(projectMemberForTest);
            
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
            (_projectMemberRepository, _userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectMemberNotExists);
    }
}