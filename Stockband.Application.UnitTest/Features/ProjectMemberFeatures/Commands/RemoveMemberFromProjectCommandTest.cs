using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.RemoveMemberFromProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectMemberFeatures.Commands;

public class RemoveMemberFromProjectCommandTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IProjectMemberRepository> _mockProjectMemberRepository;

    public RemoveMemberFromProjectCommandTest()
    {
        _mockUserRepository = UserRepositoryMock.GetUserRepositoryMock();
        _mockProjectMemberRepository = ProjectMemberRepositoryMock.GetProjectMemberRepositoryMock();
    }

    [Fact]
    public async Task RemoveMemberFromProjectCommand_ShouldBeSuccess()
    {
        const int testingProjectOwnerId = 1;
        const int testingProjectId = 1;
        const int testingMemberId = 3;
        
        List<ProjectMember> projectMembersBeforeRemove = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            ProjectOwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<ProjectMember> projectMembersAfterRemove = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        projectMembersAfterRemove.Count.ShouldBe(projectMembersBeforeRemove.Count-1);
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
    }
    
    [Fact]
    public async Task RemoveMemberFromProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingProjectOwnerId = 1000;
        const int testingProjectId = 1;
        const int testingMemberId = 1;
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            ProjectOwnerId = testingProjectOwnerId,
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
        int testingProjectOwnerId, int testingProjectId, int testingMemberId)
    {
        RemoveMemberFromProjectCommandHandler handler = new RemoveMemberFromProjectCommandHandler
        (
            _mockProjectMemberRepository.Object
        );

        RemoveMemberFromProjectCommand command = new RemoveMemberFromProjectCommand
        {
            ProjectOwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectMemberNotExists);
    }
}