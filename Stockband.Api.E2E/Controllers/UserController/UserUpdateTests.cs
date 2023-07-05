using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.Dtos.User;
using Stockband.Api.E2E.Builders;
using Stockband.Domain;
using Stockband.Domain.Common;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class UserUpdateTests:BaseTest
{
    private UserBuilder _userBuilder = null!;
    
    private const string TestingUri = "/user/update";

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
    }
    
    [Test]
    public async Task UserUpdate_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 54436;
        const string testingUpdateUsername = "testUsername";
        const string testingUpdateEmail = "test@gmail.com";
        
        await _userBuilder
            .Build(testingUserId);
        
        UpdateUserDto dto = new UpdateUserDto(testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, GetUserJwtToken(testingUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(true);
            response.Errors.Count.ShouldBe(0);
        });
    }

    [Test]
    public async Task UserUpdate_RequestedUserIsNotAdmin_BaseErrorCodeShouldBe_UserUnauthorizedOperation()
    {
        //Arrange
        const int testingUserId = 2600;
        const string testingUpdateUsername = "updateUsername";
        const string testingUpdateEmail = "update@gmail.com";
        
        UpdateUserDto dto = new UpdateUserDto(testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        const int testingRequestedId = 3200;
        const string testingRequestedUsername = "requestedUser";
        const string testingRequestedEmail = "requested@gmail.com";

        await _userBuilder
            .Build(userId:testingRequestedId, username:testingRequestedUsername, email:testingRequestedEmail);

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, GetUserJwtToken(testingRequestedId));
        
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
    public async Task UserUpdate_UpdatedUserNotExists_BaseErrorCodeShouldBe_UserNotExists()
    {
        //Arrange
        const int testingUserId = 1200;
        const string testingUpdateUsername = "updateUsername";
        const string testingUpdateEmail = "update@gmail.com";
        
        UpdateUserDto dto = new UpdateUserDto(testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        const int testingRequestedId = 2500;
        const string testingRequestedUsername = "requestedUser";
        const string testingRequestedEmail = "requested@gmail.com";

        await _userBuilder
            .Build(userId:testingRequestedId, username:testingRequestedUsername, 
                email:testingRequestedEmail, userRole: UserRole.Admin);

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, GetAdminJwtToken(testingRequestedId));
        
        //Assert

        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotExists);
        });
    }

    [Test]
    public async Task UserUpdate_ProvidedEmailAlreadyExists_BaseErrorCodeShouldBe_UserEmailAlreadyExists()
    {
        //Arrange
        const int testingUserId = 1200;
        string testingUpdateUsername = "updateUsername";
        string testingUpdateEmail = "existing@gmail.com";

        await _userBuilder
            .Build(userId:6500, username:"test username", email:testingUpdateEmail);
        
        await _userBuilder
            .Build(userId:testingUserId);
        
        UpdateUserDto dto = new UpdateUserDto(testingUserId, testingUpdateUsername, testingUpdateEmail);

        //Act
        HttpResponseModule responseModule = ActResponseModule(dto, GetUserJwtToken(testingUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserEmailAlreadyExists);
        });
    }

    private HttpResponseModule ActResponseModule
        (UpdateUserDto dto, string jwtToken)
    {
        return HttpHost
            .Put
            .Url(TestingUri)
            .WithJwtToken(jwtToken)
            .Json(dto)
            .Send()
            .Response;
    }
}