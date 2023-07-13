using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.ProjectMemberFeatures.Queries.GetAllProjectMembers;
using Stockband.Domain.Common;

namespace Stockband.Api.E2E.Controllers.ProjectMemberController;

[TestFixture]
public class GetAllProjectMembersTests:BaseTest
{
    private ProjectBuilder _projectBuilder = null!;
    private UserBuilder _userBuilder = null!;
    private ProjectMemberBuilder _projectMemberBuilder = null!;
    
    private static readonly Func<int, string> TestingUri = id => $"/projectMember/{id}";
    
    [SetUp]
    public void SetUp()
    {
        _projectBuilder = new ProjectBuilder(Context);
        _userBuilder = new UserBuilder(Context);
        _projectMemberBuilder = new ProjectMemberBuilder(Context);
    }

    [Test]
    public async Task GetAllProjectMembers_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingRequestedUserId = 200;
        await _userBuilder
            .Build(userId: testingRequestedUserId);

        const int testingProjectId = 500;
        await _projectBuilder
            .Build(projectId: testingProjectId, ownerProjectId: testingRequestedUserId);

        await _projectMemberBuilder
            .AttachManyMembersToProjectId(1200, testingProjectId, new[] { 600, 200, 800 });

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(testingProjectId, GetUserJwtToken(99541));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse<List<GetAllProjectMembersQueryViewModel>>>(response =>
        {
            response.Success.ShouldBe(true);
            response.Errors.Count.ShouldBe(0);
            response.Result.Count.ShouldBeGreaterThan(0);
        });
    }

    [Test]
    public async Task GetAllProjectMembers_ProjectWithoutMembers_ResultCount_ShouldBeZero()
    {
        //Arrange
        const int testingRequestedUserId = 200;
        await _userBuilder
            .Build(userId: testingRequestedUserId);

        const int testingProjectId = 500;
        await _projectBuilder
            .Build(projectId: testingProjectId, ownerProjectId: testingRequestedUserId);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(testingProjectId, GetUserJwtToken(8120));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse<List<GetAllProjectMembersQueryViewModel>>>(response =>
        {
            response.Success.ShouldBe(true);
            response.Errors.Count.ShouldBe(0);
            response.Result.Count.ShouldBe(0);
        });
    }
    
    private HttpResponseModule ActResponseModule(int projectId, string jwtToken)
    {
        return HttpHost
            .Get
            .WithJwtToken(jwtToken)
            .Url(TestingUri(projectId))
            .Send()
            .Response;
    }
}