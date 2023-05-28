using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.CreateProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectFeatures.Commands;

public class CreateProjectCommandTest
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;

    public CreateProjectCommandTest()
    {
        _userRepositoryMock = UserRepositoryMock.GetUserRepositoryMock();
        _projectRepositoryMock = ProjectRepositoryMock.GetProjectRepositoryMock();
    }

    [Fact]
    public async Task CreateProjectCommand_ShouldBeSuccess()
    {
        const string testingProjectName = "test project name";
        const string testingProjectDescription = "testing project description";
        const int testingProjectUserId = 1;
        
        List<Project> projectMocksBeforeCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object);

        CreateProjectCommand command = new CreateProjectCommand
        {
            OwnerId = testingProjectUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectMocksAfterCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        projectMocksAfterCommand.Count.ShouldBe(projectMocksBeforeCommand.Count+1);
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
    }
    
    [Fact]
    public async Task CreateProjectCommand_ProjectIsAlreadyCreated_ShouldNotBeSuccess()
    {
        const string testingProjectName = "Project test 2";
        const string testingProjectDescription = "testing project description";
        const int testingProjectUserId = 1;

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object);

        CreateProjectCommand command = new CreateProjectCommand
        {
            OwnerId = testingProjectUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectAlreadyCreated);
    }
    
    [Fact]
    public async Task CreateProjectCommand_OwnerNotExists_ShouldNotBeSuccess()
    {
        const string testingProjectName = "test project name";
        const string testingProjectDescription = "testing project description";
        const int testingProjectUserId = 500;
        
        List<Project> projectMocksBeforeCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object);

        CreateProjectCommand command = new CreateProjectCommand
        {
            OwnerId = testingProjectUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectMocksAfterCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        projectMocksAfterCommand.Count.ShouldBe(projectMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.Last().Code.ShouldBe(BaseErrorCode.UserNotExists);
    }
    
    [Fact]
    public async Task CreateProjectCommand_InvalidProjectName_ShouldNotBeSuccess()
    {
        const string testingProjectName = 
            "test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name";
        const string testingProjectDescription = "testing project description";
        const int testingProjectUserId = 1;
        
        List<Project> projectMocksBeforeCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object);

        CreateProjectCommand command = new CreateProjectCommand
        {
            OwnerId = testingProjectUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectMocksAfterCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        projectMocksAfterCommand.Count.ShouldBe(projectMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    
    [Fact]
    public async Task CreateProjectCommand_InvalidDescriptionName_ShouldNotBeSuccess()
    {
        const string testingProjectName = "test project name";
        const string testingProjectDescription = 
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description";
        const int testingProjectUserId = 1;
        
        List<Project> projectMocksBeforeCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object);

        CreateProjectCommand command = new CreateProjectCommand
        {
            OwnerId = testingProjectUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectMocksAfterCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        projectMocksAfterCommand.Count.ShouldBe(projectMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    [Fact]
    public async Task CreateProjectCommand_InvalidProjectNameAndInvalidDescriptionName_ShouldNotBeSuccess()
    {
        const string testingProjectName =
            "test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name";
        const string testingProjectDescription = 
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description " +
            "testing project description testing project description";
        const int testingProjectUserId = 1;
        
        List<Project> projectMocksBeforeCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();

        CreateProjectCommandHandler handler =
            new CreateProjectCommandHandler(_projectRepositoryMock.Object, _userRepositoryMock.Object);

        CreateProjectCommand command = new CreateProjectCommand
        {
            OwnerId = testingProjectUserId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectMocksAfterCommand = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        projectMocksAfterCommand.Count.ShouldBe(projectMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(2);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        response.Errors.Last().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}