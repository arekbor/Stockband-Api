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

    public UpdateProjectCommandTest()
    {
        _projectRepositoryMock = ProjectRepositoryMock.GetProjectRepositoryMock();
    }

    [Fact]
    public async Task UpdateProjectCommand_ShouldBeSuccess()
    {
        const int testingUserId = 1;
        const int testingProjectId = 1;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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

        project.Owner.Id.ShouldBe(testingUserId);
        project.OwnerId.ShouldBe(testingUserId);
        project.Description.ShouldBe(testingProjectDescription);
        project.Name.ShouldBe(testingProjectName);
        project.Id.ShouldBe(testingProjectId);
    }
    [Fact]
    public async Task UpdateProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingUserId = 2;
        const int testingProjectId = 1;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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
        const int testingUserId = 2;
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
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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
        const int testingUserId = 2;
        const int testingProjectId = 1;
        const string testingProjectName = 
            "test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name test project " +
            "name test project name test project name test project name";
        const string testingProjectDescription = "test description";

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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
        const int testingUserId = 2;
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

        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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
        const int testingUserId = 1;
        const int testingProjectId = 1000;
        const string testingProjectName = "test name";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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
        const int testingUserId = 1;
        const int testingProjectId = 1;
        const string testingProjectName = "Project test 1";
        const string testingProjectDescription = "test description";
        
        UpdateProjectCommandHandler handler = new UpdateProjectCommandHandler(_projectRepositoryMock.Object);
        UpdateProjectCommand command = new UpdateProjectCommand
        {
            OwnerId = testingUserId,
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