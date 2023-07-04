using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.Dtos.Project;
using Stockband.Api.E2E.Builders;
using Stockband.Domain;
using Stockband.Domain.Common;

namespace Stockband.Api.E2E.Controllers.ProjectController;

public class CreateProjectTests:BaseTest
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
    public async Task CreateProject_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const string testingProjectName = "test project";
        const string testingProjectDescription = "testing project description";
        
        const int testingRequestedUserId = 5400;
        await _userBuilder
            .Build(userId: testingRequestedUserId);

        CreateProjectDto dto = new CreateProjectDto(testingProjectName, testingProjectDescription);

        //Act
        HttpResponseModule responseModule = ActResponseModule(dto, GetUserJwtToken(testingRequestedUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(true);
            response.Errors.Count.ShouldBe(0);
        });
    }

    [Test]
    [TestCaseSource(nameof(_wrongProjectNameOrDescriptionSchemeTestCases))]
    public void CreateProject_WrongProjectNameOrDescriptionScheme_BaseErrorCodeShouldBe_FluentValidationCode
        (string projectName, string description)
    {
        //Arrange
        CreateProjectDto dto = new CreateProjectDto(projectName, description);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto, GetUserJwtToken(1000));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        });
    }

    [Test]
    public async Task CreateProject_ProvidedProjectNameIsUsed_BaseErrorCodeShouldBe_ProjectAlreadyCreated()
    {
        //Arrange
        const string testingExistingProjectName = "test existing project name";
        await _projectBuilder.Build(projectId:5400, projectName:testingExistingProjectName);
        
        CreateProjectDto dto = new CreateProjectDto(testingExistingProjectName, String.Empty);

        const int requestedUserId = 2500;
        await _userBuilder.Build(userId:requestedUserId);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto, GetUserJwtToken(requestedUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.ProjectAlreadyCreated);
        });
    }

    private HttpResponseModule ActResponseModule(CreateProjectDto dto, string jwtToken)
    {
        return HttpHost
            .Post
            .WithJwtToken(jwtToken)
            .Url(TestingUri)
            .Json(dto)
            .Send()
            .Response;
    }
    
    private static object[] _wrongProjectNameOrDescriptionSchemeTestCases =
    {
        new object[] {String.Empty, "testing project description"},
        new object[] {"test project name", string.Join(string.Empty, new string('#', 1200))},
        new object[] {string.Join(string.Empty, new string('#', 200)), "testing project description"}
    };
}