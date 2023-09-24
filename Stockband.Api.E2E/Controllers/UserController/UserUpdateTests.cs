using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.UserFeatures.Commands.UpdateUser;
using Stockband.Domain.Enums;
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
        
        UpdateUserCommand command = new UpdateUserCommand
            (testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(command, GetUserJwtToken(testingUserId));

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

        await _userBuilder.Build(userId: testingUserId);
        
        UpdateUserCommand command = new UpdateUserCommand
            (testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        const int testingRequestedId = 3200;
        const string testingRequestedUsername = "requestedUser";
        const string testingRequestedEmail = "requested@gmail.com";

        await _userBuilder
            .Build(userId:testingRequestedId, username:testingRequestedUsername, email:testingRequestedEmail);

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(command, GetUserJwtToken(testingRequestedId));
        
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
        
        UpdateUserCommand command = new UpdateUserCommand
            (testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        const int testingRequestedId = 2500;
        const string testingRequestedUsername = "requestedUser";
        const string testingRequestedEmail = "requested@gmail.com";

        await _userBuilder
            .Build(userId:testingRequestedId, username:testingRequestedUsername, 
                email:testingRequestedEmail, userRole: UserRole.Admin);

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(command, GetAdminJwtToken(testingRequestedId));
        
        //Assert

        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotFound);
        });
    }

    [Test]
    public async Task UserUpdate_ProvidedEmailAlreadyExists_BaseErrorCodeShouldBe_UserEmailAlreadyExists()
    {
        //Arrange
        const int testingUserId = 1200;
        const int currentUserId = 600;
        
        string testingUpdateUsername = "updateUsername";
        string testingUpdateEmail = "existing@gmail.com";
        
        await _userBuilder
            .Build(userId: currentUserId);
        await _userBuilder
            .Build(userId:testingUserId, email:testingUpdateEmail);
        
        UpdateUserCommand command = new UpdateUserCommand
            (testingUserId, testingUpdateUsername, testingUpdateEmail);

        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetAdminJwtToken(currentUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserEmailAlreadyExists);
        });
    }
    
    [Test]
    public async Task UserUpdate_SameEmailAsUser_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 1200;
        string testingUpdateUsername = "updateUsername";
        string testingUpdateEmail = "existing@gmail.com";
        
        await _userBuilder
            .Build(userId:testingUserId, email:testingUpdateEmail);
        
        UpdateUserCommand command = new UpdateUserCommand
            (testingUserId, testingUpdateUsername, testingUpdateEmail);

        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
    }
    
    private HttpResponseModule ActResponseModule
        (UpdateUserCommand command, string jwtToken)
    {
        return HttpHost
            .Put
            .Url(TestingUri)
            .WithJwtToken(jwtToken)
            .Json(command)
            .Send()
            .Response;
    }
}