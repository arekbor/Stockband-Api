using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.ProjectFeatures.Commands;

public class CreateProjectCommandTest:BaseTest
{
    [Test]
    public async Task CreateProjectCommand_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);

        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(5);
        const int testingRequestedUserId = 1052;

        User userTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userTest);

        CreateProjectCommand command = new CreateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(projectRepository, userRepository);

        //Act
        IEnumerable<Project> listOfProjectsBeforeHandler = await projectRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<Project> listOfProjectsAfterHandler = await projectRepository.GetAllAsync();

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        listOfProjectsAfterHandler.ToList().Count.ShouldBe(listOfProjectsBeforeHandler.ToList().Count + 1);
    }

    [Test]
    public async Task CreateProjectCommand_ProjectIsAlreadyCreated_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);

        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(5);
        const int testingRequestedUserId = 5623;

        CreateProjectCommand command = new CreateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(user);

        Project project = Builder<Project>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Name = testingProjectName)
            .With(x => x.Description = testingProjectDescription)
            .Build();
        await projectRepository.AddAsync(project);

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(projectRepository, userRepository);

        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectAlreadyCreated);
    }

    [Test]
    public async Task CreateProjectCommand_RequestedUserNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);

        string testingProjectName = new Faker().Lorem.Sentence(1);
        string testingProjectDescription = new Faker().Lorem.Sentence(5);
        const int testingRequestedUserId = 5000;

        CreateProjectCommand command = new CreateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(projectRepository, userRepository);

        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.Last().Code.ShouldBe(BaseErrorCode.UserNotExists);
    }
    
    [Test]
    [TestCaseSource(typeof(ProjectFeaturesTestCasesData), nameof(ProjectFeaturesTestCasesData.InvalidProjectNamesOrDescriptionsCases))]
    public async Task CreateProjectCommand_InvalidProjectNameOrDescription_ResponseShouldBeNotSuccess
        (string testingProjectName, string testingProjectDescription)
    {
        //arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 1323123215;
        
        CreateProjectCommand command = new CreateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(projectRepository, userRepository);
        
        //act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    
    [Test]
    [TestCaseSource(typeof(ProjectFeaturesTestCasesData), nameof(ProjectFeaturesTestCasesData.InvalidProjectNamesAndDescriptionsCases))]
    public async Task CreateProjectCommand_InvalidProjectNameAndDescription_ResponseShouldBeNotSuccess
        (string testingProjectName, string testingProjectDescription)
    {
        //arrange
        UserRepository userRepository = new UserRepository(Context);
        ProjectRepository projectRepository = new ProjectRepository(Context);
        
        const int testingRequestedUserId = 943445511;
        
        CreateProjectCommand command = new CreateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(projectRepository, userRepository);
        
        //act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(2);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        response.Errors.Last().Code.ShouldBe(BaseErrorCode.FluentValidationCode);

    }
}