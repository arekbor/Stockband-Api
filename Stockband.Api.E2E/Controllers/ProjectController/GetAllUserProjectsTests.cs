using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.ProjectFeatures.Queries.GetAllUserProjects;
using Stockband.Domain.Common;

namespace Stockband.Api.E2E.Controllers.ProjectController;

public class GetAllUserProjectsTests:BaseTest
{
    private static readonly Func<int, string> TestingUri = userId => $"/projects/{userId}";

    private UserBuilder _userBuilder = null!;
    private ProjectBuilder _projectBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
        _projectBuilder = new ProjectBuilder(Context);
    }

    [Test]
    public async Task GetAllUserProjects_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int sizeProjects = 10;
        
        const int testingUserId = 500;
        await _userBuilder
            .Build(userId: testingUserId);

        await _projectBuilder
            .BuildMany(projectOwnerId: testingUserId, size:sizeProjects);

        //Act
        HttpResponseModule responseModule = ActResponseModule(testingUserId, GetUserJwtToken(5600));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse<List<GetAllUserProjectsQueryViewModel>>>(response =>
        {
            response.Result.Count.ShouldBe(sizeProjects);
            response.Errors.Count.ShouldBe(0);
            response.Success.ShouldBe(true);
        });
    }
    
    [Test]
    public async Task GetAllUserProjects_BaseResponse_ResultCount_ShouldBeZero()
    {
        HttpResponseModule responseModule = ActResponseModule(7500, GetUserJwtToken(5600));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse<List<GetAllUserProjectsQueryViewModel>>>(response =>
        {
            response.Result.Count.ShouldBe(0);
            response.Errors.Count.ShouldBe(0);
            response.Success.ShouldBe(true);
        });
    }

    private HttpResponseModule ActResponseModule(int userId, string jwtToken)
    {
        return HttpHost
            .Get
            .WithJwtToken(jwtToken)
            .Url(TestingUri(userId))
            .Send()
            .Response;
    }
}