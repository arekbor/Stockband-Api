using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.ProjectMemberFeatures.Commands;

public class AddProjectMemberToProjectCommandTest:BaseTest
{
    [Test]
    public async Task AddProjectMemberToProject_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 70054;
        const int testingProjectId = 5993;
        const int testingMemberId = 9932;
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

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
        await userRepository.AddRangeAsync(usersForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .With(x => x.OwnerId = testingRequestedUserId)
            .Build();
        await projectRepository.AddAsync(projectForTest);

        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        IEnumerable<ProjectMember> listOfProjectMembersBeforeHandler = await projectMemberRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<ProjectMember> listOfProjectMembersAfterHandler = await projectMemberRepository.GetAllAsync();
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        listOfProjectMembersAfterHandler.Count().ShouldBe(listOfProjectMembersBeforeHandler.Count()+1);
    }

    [Test]
    public async Task AddProjectMemberToProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 80443;
        const int testingProjectId = 90223;
        const int testingMemberId = 73321;
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        List<User> usersForTest = Builder<User>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingRequestedUserId)
            .With(x => x.Role = UserRole.Admin)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingMemberId)
            .Build()
            .ToList();
        await userRepository.AddRangeAsync(usersForTest);

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await projectRepository.AddAsync(projectForTest);
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        IEnumerable<ProjectMember> listOfProjectMembersBeforeHandler = await projectMemberRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<ProjectMember> listOfProjectMembersAfterHandler = await projectMemberRepository.GetAllAsync();

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        listOfProjectMembersAfterHandler.Count().ShouldBe(listOfProjectMembersBeforeHandler.Count()+1);
    }

    [Test]
    public async Task AddProjectMemberToProjectCommand_ProjectMemberAlreadyCreated_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 9009335;
        const int testingProjectId = 43325654;
        const int testingMemberId = 97741231;
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .With(x => x.OwnerId = testingRequestedUserId)
            .Build();
        await projectRepository.AddAsync(projectForTest);

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
        await userRepository.AddRangeAsync(usersForTest);

        ProjectMember projectMemberForTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.MemberId = testingMemberId)
            .With(x => x.ProjectId = testingProjectId)
            .Build();
        await projectMemberRepository.AddAsync(projectMemberForTest);

        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectMemberAlreadyCreated);
    }

    [Test]
    public async Task AddProjectMemberToProjectCommand_RequestedUserIsNotOwner_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 1543999814;
        const int testingProjectId = 1543999812;
        const int testingMemberId = 1543999813;
        
        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await projectRepository.AddAsync(projectForTest);

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userForTest);

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }

    [Test]
    public async Task AddProjectMemberToProjectCommand_ProjectNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 905;
        const int testingProjectId = 1012;
        const int testingMemberId = 2542;
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }
    
    [Test]
    public async Task AddProjectMemberToProjectCommand_MemberNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 54123;
        const int testingProjectId = 10331512;
        const int testingMemberId = 612542;
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };
        
        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .With(x => x.OwnerId = testingRequestedUserId)
            .Build();
        await projectRepository.AddAsync(projectForTest);

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userForTest);
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.MemberForProjectMemberNotExists);
    }

    [Test]
    public async Task AddProjectMemberToProjectCommand_MemberIdIsEqualToOwnerId_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 888712;
        const int testingProjectId = 1333476;
        const int testingMemberId = 888712;
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };
        
        Project projectForTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .With(x => x.OwnerId = testingRequestedUserId)
            .Build();
        await projectRepository.AddAsync(projectForTest);

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userForTest);
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
            (userRepository, projectRepository, projectMemberRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserOperationRestricted);
    }
}