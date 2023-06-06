using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Commands.AddProjectMemberToProject;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectMemberFeatures.Commands;

public class AddProjectMemberToProjectCommandTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<IProjectMemberRepository> _mockProjectMemberRepository;

    public AddProjectMemberToProjectCommandTest()
    {
        _mockUserRepository = UserRepositoryMock.GetUserRepositoryMock();
        _mockProjectRepository = ProjectRepositoryMock.GetProjectRepositoryMock();
        _mockProjectMemberRepository = ProjectMemberRepositoryMock.GetProjectMemberRepositoryMock();
    }

    [Fact]
    public async Task AddProjectMemberToProjectCommand_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 3;
        const int testingMemberId = 3;

        List<ProjectMember> projectMembersBeforeAdd = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<ProjectMember> projectMembersAfterAdd = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        projectMembersAfterAdd.Count.ShouldBe(projectMembersBeforeAdd.Count+1);
    }
    
    [Fact]
    public async Task AddProjectMemberToProjectCommand_RequestedUserIsNotOwnerButIsAdmin_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingProjectId = 3;
        const int testingMemberId = 3;
        
        Project? testingProject = _mockProjectRepository.Object.GetByIdAsync(testingProjectId).Result;
        if (testingProject == null)
        {
            throw new ObjectNotFound(typeof(Project), testingProjectId);
        }

        List<ProjectMember> projectMembersBeforeAdd = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        List<ProjectMember> projectMembersAfterAdd = _mockProjectMemberRepository.Object.GetAllAsync().Result.ToList();
        
        User? requestedUser = _mockUserRepository.Object.GetByIdAsync(testingRequestedUserId).Result;
        if (requestedUser == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        projectMembersAfterAdd.Count.ShouldBe(projectMembersBeforeAdd.Count+1);
        testingProject.OwnerId.ShouldNotBe(testingRequestedUserId);
        
        requestedUser.Role.ShouldBe(UserRole.Admin);
    }
    
    [Fact]
    public async Task AddProjectMemberToProjectCommand_ProjectMemberAlreadyCreated_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 1;
        const int testingMemberId = 2;
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectMemberAlreadyCreated);
    }

    [Fact]
    public async Task AddProjectMemberToProjectCommand_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 4;
        const int testingProjectId = 3;
        const int testingMemberId = 2;
            
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
    
    [Fact]
    public async Task AddProjectMemberToProjectCommand_ProjectNotExists_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingProjectId = 3000;
        const int testingMemberId = 2;
            
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
    }
    
    [Fact]
    public async Task AddProjectMemberToProjectCommand_MemberNotExists_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 3;
        const int testingMemberId = 2000;
            
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );

        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingMemberId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotExists);
    }

    [Fact]
    public async Task AddProjectMemberToProjectCommand_MemberIdIsSameAsOwnerId_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 2;
        const int testingProjectId = 3;
        
        AddProjectMemberToProjectCommandHandler handler = new AddProjectMemberToProjectCommandHandler
        (
            _mockUserRepository.Object,
            _mockProjectRepository.Object,
            _mockProjectMemberRepository.Object
        );
        
        AddProjectMemberToProjectCommand command = new AddProjectMemberToProjectCommand
        {
            RequestedUserId = testingRequestedUserId,
            ProjectId = testingProjectId,
            MemberId = testingRequestedUserId
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserOperationRestricted);
    }
}