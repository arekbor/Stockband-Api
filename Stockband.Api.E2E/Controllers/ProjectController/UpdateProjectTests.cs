using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.ProjectFeatures.Commands.UpdateProject;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;

namespace Stockband.Api.E2E.Controllers.ProjectController;

[TestFixture]
public class UpdateProjectTests:BaseTest
{
    private UserBuilder _userBuilder = null!;
    private ProjectBuilder _projectBuilder = null!;
    private const string TestingUri = "/project";

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
        _projectBuilder = new ProjectBuilder(Context);
    }

    [Test]
    public async Task UpdateProject_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingRequestedUserId = 4233;
        await _userBuilder
            .Build(testingRequestedUserId);
        
        const int testingUpdateProjectId = 5400;
        const string testingUpdateProjectName = "updated project name";
        const string testingUpdateProjectDescription = "updated project description";

        await _projectBuilder
            .Build(projectId:testingUpdateProjectId, ownerProjectId: testingRequestedUserId);

        UpdateProjectCommand command = new UpdateProjectCommand
            (testingUpdateProjectId, testingUpdateProjectName, testingUpdateProjectDescription);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingRequestedUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(true);
            response.Errors.Count.ShouldBe(0);
        });
    }

    [Test]
    public async Task UpdateProject_ProvidedProjectIdNotExists_BaseErrorCodeShouldBe_ProjectNotExists()
    {
        //Arrange
        const int testingRequestedUserId = 5669;
        await _userBuilder
            .Build(testingRequestedUserId);
        
        const int testingUpdateProjectId = 5400;
        const string testingUpdateProjectName = "updated project name";
        const string testingUpdateProjectDescription = "updated project description";
        
        UpdateProjectCommand command = new UpdateProjectCommand
            (testingUpdateProjectId, testingUpdateProjectName, testingUpdateProjectDescription);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingRequestedUserId));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectNotFound);
        });
    }

    [Test]
    public async Task UpdateProject_RequestedUserIsNotOwner_BaseErrorCodeShouldBe_UserUnauthorizedOperation()
    {
        //Arrange
        const int testingRequestedUserId = 7553;
        await _userBuilder
            .Build(testingRequestedUserId);
        
        const int testingUpdateProjectId = 9887;
        const string testingUpdateProjectName = "updated project name";
        const string testingUpdateProjectDescription = "updated project description";

        await _projectBuilder
            .Build(projectId:testingUpdateProjectId);

        UpdateProjectCommand command = new UpdateProjectCommand
            (testingUpdateProjectId, testingUpdateProjectName, testingUpdateProjectDescription);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingRequestedUserId));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
        });
    }

    [Test]
    public async Task UpdateProject_ProvidedProjectNameIsUsed_BaseErrorCodeShouldBe_ProjectAlreadyCreated()
    {
        //Arrange
        const int testingRequestedUserId = 8954;
        await _userBuilder
            .Build(testingRequestedUserId);
        
        const int testingUpdateProjectId = 12334;
        const string testingUpdateProjectName = "updated project name";
        const string testingUpdateProjectDescription = "updated project description";

        await _projectBuilder
            .Build(projectId:testingUpdateProjectId, ownerProjectId: testingRequestedUserId);

        await _projectBuilder
            .Build(projectId: 7756, projectName: testingUpdateProjectName);

        UpdateProjectCommand command = new UpdateProjectCommand
            (testingUpdateProjectId, testingUpdateProjectName, testingUpdateProjectDescription);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingRequestedUserId));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectAlreadyCreated);
        });
    }
    
    private HttpResponseModule ActResponseModule(UpdateProjectCommand command, string jwtToken)
    {
        return HttpHost
            .Put
            .WithJwtToken(jwtToken)
            .Url(TestingUri)
            .Json(command)
            .Send()
            .Response;
    }
}