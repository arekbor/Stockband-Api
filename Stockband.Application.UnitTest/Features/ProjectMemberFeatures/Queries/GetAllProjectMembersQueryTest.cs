using Moq;
using Shouldly;
using Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Xunit;

namespace Stockband.Application.UnitTest.Features.ProjectMemberFeatures.Queries;

public class GetAllProjectMembersQueryTest
{
    private readonly Mock<IProjectMemberRepository> _mockProjectMemberRepository;
    private readonly Mock<IProjectRepository> _mockProjectRepository;

    public GetAllProjectMembersQueryTest()
    {
        _mockProjectMemberRepository = ProjectMemberRepositoryMock.GetProjectMemberRepositoryMock();
        _mockProjectRepository = ProjectRepositoryMock.GetProjectRepositoryMock();
    }

    [Fact]
    public async Task GetAllProjectMembersQuery_ShouldBeSuccess()
    {
        const int testingProjectId = 1;
        const int testingProjectOwnerId = 1;
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_mockProjectMemberRepository.Object, _mockProjectRepository.Object);

        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            ProjectOwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId
        };

        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        response.Result.Count.ShouldBeGreaterThan(0);
        response.Result.ShouldNotBeNull();
    }
    
    [Fact]
    public async Task GetAllProjectMembersQuery_InvalidProjectId_ShouldNotBeSuccess()
    {
        const int testingProjectId = 1000;
        const int testingProjectOwnerId = 1;
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_mockProjectMemberRepository.Object, _mockProjectRepository.Object);

        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            ProjectOwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId
        };

        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotExists);
        response.Result.ShouldBeNull();
    }
    
    [Fact]
    public async Task GetAllProjectMembersQuery_InvalidProjectOwner_ShouldNotBeSuccess()
    {
        const int testingProjectId = 1;
        const int testingProjectOwnerId = 5;
        
        GetAllProjectMembersQueryHandler handler =
            new GetAllProjectMembersQueryHandler(_mockProjectMemberRepository.Object, _mockProjectRepository.Object);

        GetAllProjectMembersQuery query = new GetAllProjectMembersQuery
        {
            ProjectOwnerId = testingProjectOwnerId,
            ProjectId = testingProjectId
        };

        BaseResponse<List<GetAllProjectMembersQueryViewModel>> response = await handler.Handle(query, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
        response.Result.ShouldBeNull();
    }
}