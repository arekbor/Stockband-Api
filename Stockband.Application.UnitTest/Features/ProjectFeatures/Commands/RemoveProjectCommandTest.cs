using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectFeatures.Commands;

public class RemoveProjectCommandTest
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IProjectMemberRepository> _projectMemberRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public RemoveProjectCommandTest()
    {
        _projectRepositoryMock = ProjectRepositoryMock.GetProjectRepositoryMock();
        _projectMemberRepositoryMock = ProjectMemberRepositoryMock.GetProjectMemberRepositoryMock();
        _userRepositoryMock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task RemoveProjectCommand_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingProjectId = 7;

        List<Project> projectsBeforeRemove = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            _projectRepositoryMock.Object, _projectMemberRepositoryMock.Object, _userRepositoryMock.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectsAfterRemove = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        projectsAfterRemove.Count.ShouldBe(projectsBeforeRemove.Count-1);
    }
    
    [Fact]
    public async Task RemoveProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingProjectId = 2;
        
        Project? testingProject = _projectRepositoryMock.Object.GetByIdAsync(testingProjectId).Result;
        if (testingProject == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }

        List<Project> projectsBeforeRemove = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            _projectRepositoryMock.Object, _projectMemberRepositoryMock.Object, _userRepositoryMock.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectsAfterRemove = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        testingProject.OwnerId.ShouldNotBe(testingRequestedUserId);
        projectsAfterRemove.Count.ShouldBe(projectsBeforeRemove.Count-1);
    }
    
    [Fact]
    public async Task RemoveProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 7;
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            _projectRepositoryMock.Object, _projectMemberRepositoryMock.Object, _userRepositoryMock.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
    
    [Fact]
    public async Task RemoveProjectCommand_ProjectNotExists_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingProjectId = 7000;
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            _projectRepositoryMock.Object, _projectMemberRepositoryMock.Object, _userRepositoryMock.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }
    
    [Fact]
    public async Task RemoveProjectCommand_ProjectMembersAreAssigned_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 1;
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(
            _projectRepositoryMock.Object, _projectMemberRepositoryMock.Object, _userRepositoryMock.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserOperationRestricted);
    }
}