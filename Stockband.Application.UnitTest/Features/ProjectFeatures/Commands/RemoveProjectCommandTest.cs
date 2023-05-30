using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectFeatures.Commands.RemoveProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectFeatures.Commands;

public class RemoveProjectCommandTest
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IProjectMemberRepository> _projectMemberRepository;

    public RemoveProjectCommandTest()
    {
        _projectRepositoryMock = ProjectRepositoryMock.GetProjectRepositoryMock();
        _projectMemberRepository = ProjectMemberRepositoryMock.GetProjectMemberRepositoryMock();
    }

    [Fact]
    public async Task RemoveProjectCommand_ShouldBeSuccess()
    {
        const int testingProjectOwnerId = 5;
        const int testingProjectId = 7;

        List<Project> projectsBeforeRemove = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(_projectRepositoryMock.Object, _projectMemberRepository.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            OwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<Project> projectsAfterRemove = _projectRepositoryMock.Object.GetAllAsync().Result.ToList();
        
        projectsAfterRemove.Count.ShouldBe(projectsBeforeRemove.Count-1);
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
    }
    
    [Fact]
    public async Task RemoveProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingProjectOwnerId = 5000;
        const int testingProjectId = 7;
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(_projectRepositoryMock.Object, _projectMemberRepository.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            OwnerId = testingProjectOwnerId,
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
        const int testingProjectOwnerId = 5;
        const int testingProjectId = 7000;
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(_projectRepositoryMock.Object, _projectMemberRepository.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            OwnerId = testingProjectOwnerId,
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
        const int testingProjectOwnerId = 1;
        const int testingProjectId = 1;
        
        RemoveProjectCommandHandler handler = new RemoveProjectCommandHandler(_projectRepositoryMock.Object, _projectMemberRepository.Object);

        RemoveProjectCommand command = new RemoveProjectCommand
        {
            OwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserOperationRestricted);
    }
}