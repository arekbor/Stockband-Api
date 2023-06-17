using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.ProjectFeatures.Commands;



public class UpdateProjectCommandTest:BaseTest
{
    [Test]
    public async Task UpdateProjectCommand_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 99493211;
        const int testingProjectId = 123155433;
        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(2);
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        Project projectTest = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .With(x => x.OwnerId = testingRequestedUserId)
            .Build();
        await projectRepository.AddAsync(projectTest);
        
        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());

        Project? updatedProjectTest = await projectRepository.GetByIdAsync(testingProjectId);
        if (updatedProjectTest == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        updatedProjectTest.OwnerId.ShouldBe(testingRequestedUserId);
        updatedProjectTest.Description.ShouldBe(testingProjectDescription);
        updatedProjectTest.Name.ShouldBe(testingProjectName);
        updatedProjectTest.Id.ShouldBe(testingProjectId);
    }
    
    [Test]
    public async Task UpdateProjectCommand_RequestedUserIsNotOwnerBytIsAdmin_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 125553;
        const int testingProjectId = 6667712;
        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(2);
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
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
            .With(x => x.Role = UserRole.Admin)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());

        Project? updatedProjectTest = await projectRepository.GetByIdAsync(testingProjectId);
        if (updatedProjectTest == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        updatedProjectTest.Description.ShouldBe(testingProjectDescription);
        updatedProjectTest.Name.ShouldBe(testingProjectName);
        updatedProjectTest.Id.ShouldBe(testingProjectId);
    }

    [Test]
    public async Task UpdateProjectCommand_InvalidProjectOwner_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 112232;
        const int testingProjectId = 554212;
        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(2);
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
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
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }

    [Test]
    public async Task UpdateProjectCommand_ProjectNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 34443;
        const int testingProjectId = 33321;
        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(2);
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }

    [Test]
    public async Task UpdateProjectCommand_ProjectWithProjectNameAlreadyExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 55112;
        const int testingProjectId = 18124;
        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(2);
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        List<Project> projectsForTest = Builder<Project>
            .CreateListOfSize(5)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Name = testingProjectName)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingProjectId)
            .With(x => x.OwnerId = testingRequestedUserId)
            .Build()
            .ToList();
        await Context.Projects.AddRangeAsync(projectsForTest);
        
        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectAlreadyCreated);
    }

    [Test]
    [TestCaseSource(typeof(ProjectFeaturesTestCasesData), nameof(ProjectFeaturesTestCasesData.InvalidProjectNamesOrDescriptionsCases))]
    public async Task UpdateProjectCommand_InvalidProjectNameOrDescription_ResponseShouldBeNotSuccess
        (string testingProjectName, string testingProjectDescription)
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 432432424;
        const int testingProjectId = 3333221;
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    
    [Test]
    [TestCaseSource(typeof(ProjectFeaturesTestCasesData), nameof(ProjectFeaturesTestCasesData.InvalidProjectNamesAndDescriptionsCases))]
    public async Task UpdateProjectCommand_InvalidProjectNameAndDescription_ResponseShouldBeNotSuccess
        (string testingProjectName, string testingProjectDescription)
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 8886633;
        const int testingProjectId = 33357119;
        
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            projectRepository, userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(2);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        response.Errors.Last().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}