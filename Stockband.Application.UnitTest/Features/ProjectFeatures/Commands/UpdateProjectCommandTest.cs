using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectFeatures.Commands;

public class UpdateProjectCommandTest
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public UpdateProjectCommandTest()
    {
        _projectRepositoryMock = ProjectRepositoryMock.GetProjectRepositoryMock();
        _userRepositoryMock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task UpdateProjectCommand_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 1;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());

        Project? project = await _projectRepositoryMock.Object.GetByIdAsync(testingProjectId);
        if (project == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }

        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        project.OwnerId.ShouldBe(testingRequestedUserId);
        project.Description.ShouldBe(testingProjectDescription);
        project.Name.ShouldBe(testingProjectName);
        project.Id.ShouldBe(testingProjectId);
    }
    
    [Fact]
    public async Task UpdateProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingProjectId = 1;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        Project? testingProject = _projectRepositoryMock.Object.GetByIdAsync(testingProjectId).Result;
        if (testingProject == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());

        Project? project = await _projectRepositoryMock.Object.GetByIdAsync(testingProjectId);
        if (project == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }
        
        User? requestedUser = _userRepositoryMock.Object.GetByIdAsync(testingRequestedUserId).Result;
        if (requestedUser == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }

        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        project.Description.ShouldBe(testingProjectDescription);
        project.Name.ShouldBe(testingProjectName);
        project.Id.ShouldBe(testingProjectId);
        
        testingProject.OwnerId.ShouldNotBe(testingRequestedUserId);
        
        requestedUser.Role.ShouldBe(UserRole.Admin);
    }
    
    [Fact]
    public async Task UpdateProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 1;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
    [Fact]
    public async Task UpdateProjectCommand_InvalidDescriptionName_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 1;
        const string testingProjectName = "test name";
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
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        
    }
    [Fact]
    public async Task UpdateProjectCommand_InvalidProjectName_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 1;
        const string testingProjectName = 
            "test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name";
        const string testingProjectDescription = "test description";

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    [Fact]
    public async Task UpdateProjectCommand_InvalidProjectNameAndInvalidDescriptionName_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 1;
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

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(2);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        response.Errors.Last().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }

    [Fact]
    public async Task UpdateProjectCommand_ProjectNotExists_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 1000;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }
    
    [Fact]
    public async Task UpdateProjectCommand_ProjectAlreadyCreated_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 1;
        const string testingProjectName = "Project test 1";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(
            _projectRepositoryMock.Object, _userRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            ProjectName = testingProjectName,
            ProjectDescription = testingProjectDescription
        };

        BaseResponse response = await handler.Handle(command, new CancellationToken());
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectAlreadyCreated);
    }
}