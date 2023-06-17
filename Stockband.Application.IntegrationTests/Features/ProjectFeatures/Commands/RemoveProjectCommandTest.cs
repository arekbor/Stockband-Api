using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.ProjectFeatures.Commands;

public class RemoveProjectCommandTest:BaseTest
{
    [Test]
    public async Task RemoveProjectCommand_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 5032;
        const int testingProjectId = 7331;
        
        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };

        Project projectTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.OwnerId = testingRequestedUserId)
            .With(x => x.Id = testingProjectId)
            .Build();
        await projectRepository.AddAsync(projectTest);

        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);

        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            projectRepository, projectMemberRepository, userRepository);
        
        //Act
        IEnumerable<Project> listOfProjectsBeforeHandler = await projectRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<Project> listOfProjectsAfterHandler = await projectRepository.GetAllAsync();
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        listOfProjectsAfterHandler.ToList().Count.ShouldBe(listOfProjectsBeforeHandler.ToList().Count-1);
    }

    [Test]
    public async Task RemoveProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 5312;
        const int testingProjectId = 25523;
        
        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        Project projectTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await projectRepository.AddAsync(projectTest);

        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingRequestedUserId)
            .With(x => x.Role = UserRole.Admin)
            .Build();
        await userRepository.AddAsync(userTest);
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            projectRepository, projectMemberRepository, userRepository);
        
        //Act
        IEnumerable<Project> listOfProjectsBeforeHandler = await projectRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<Project> listOfProjectsAfterHandler = await projectRepository.GetAllAsync();
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        listOfProjectsAfterHandler.ToList().Count.ShouldBe(listOfProjectsBeforeHandler.ToList().Count-1);
    }

    [Test]
    public async Task RemoveProjectCommand_RequestedUserIsNotOwner_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 53511;
        const int testingProjectId = 71296;
        
        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        Project projectTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .Build();
        await projectRepository.AddAsync(projectTest);

        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            projectRepository, projectMemberRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }

    [Test]
    public async Task RemoveProjectCommand_ProjectNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 82333;
        const int testingProjectId = 77112;
        
        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            projectRepository, projectMemberRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }

    [Test]
    public async Task RemoveProjectCommand_ProjectMembersAreAssigned_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        ProjectMemberRepository projectMemberRepository = new ProjectMemberRepository(Context);
        
        const int testingRequestedUserId = 155441;
        const int testingProjectId = 1333166;
        
        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };
        
        Project projectTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.OwnerId = testingRequestedUserId)
            .With(x => x.Id = testingProjectId)
            .Build();
        await projectRepository.AddAsync(projectTest);

        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);

        ProjectMember projectMemberTest = Builder<ProjectMember>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.ProjectId = testingProjectId)
            .Build();
        await projectMemberRepository.AddAsync(projectMemberTest);

        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            projectRepository, projectMemberRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserOperationRestricted);
    }
}