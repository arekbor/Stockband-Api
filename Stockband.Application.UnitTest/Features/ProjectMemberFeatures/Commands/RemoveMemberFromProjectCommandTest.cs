using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectMemberFeatures.Commands;

public class RemoveMemberFromProjectCommandTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IProjectMemberRepository> _mockProjectMemberRepository;
    private readonly Mock<IProjectRepository> _mockProjectRepository;

    public RemoveMemberFromProjectCommandTest()
    {
        _mockUserRepository = UserRepositoryMock.GetUserRepositoryMock();
        _mockProjectMemberRepository = ProjectMemberRepositoryMock.GetProjectMemberRepositoryMock();
        _mockProjectRepository = ProjectRepositoryMock.GetProjectRepositoryMock();
    }

    [Fact]
    public async Task RemoveMemberFromProjectCommand_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 1;
        const int testingMemberId = 3;
        
        List<ProjectMember> projectMembersBeforeRemove = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object, _mockUserRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<ProjectMember> projectMembersAfterRemove = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        projectMembersAfterRemove.Count.ShouldBe(projectMembersBeforeRemove.Count-1);
    }
    
    [Fact]
    public async Task RemoveMemberFromProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingProjectId = 1;
        const int testingMemberId = 3;
        
        Project? testingProject = _mockProjectRepository.Object.GetByIdAsync(testingProjectId).Result;
        if (testingProject == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }
        
        List<ProjectMember> projectMembersBeforeRemove = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object, _mockUserRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<ProjectMember> projectMembersAfterRemove = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        projectMembersAfterRemove.Count.ShouldBe(projectMembersBeforeRemove.Count-1);
        testingProject.OwnerId.ShouldNotBe(testingRequestedUserId);
    }
    
    [Fact]
    public async Task RemoveMemberFromProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 1;
        const int testingMemberId = 1;
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object, _mockUserRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
    
    [Theory]
    [InlineData(2, 1323, 5523)]
    [InlineData(2, 1, 5523)]
    [InlineData(2, 11323, 1)]
    public async Task RemoveMemberFromProjectCommand_ProjectMemberNotExists_ShouldNotBeSuccess(
        int testingRequestedUserId, int testingProjectId, int testingMemberId)
    {
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object, _mockUserRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectMemberNotExists);
    }
}