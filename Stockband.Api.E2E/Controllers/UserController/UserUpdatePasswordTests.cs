using System.Net;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;
using Stockband.Domain.Enums;
using Stockband.Domain.Common;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class UserUpdatePasswordTests:BaseTest
{
    private UserBuilder _userBuilder = null!;
    private const string TestingUri = "/user/password";

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
    }
    
    [Test]
    public async Task UserUpdatePassword_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 33412;
        const string testingEmail = "test@gmail.com";
        const string testingUsername = "testUsername";
        const UserRole testingUserRole = UserRole.User;
        
        const string testingCurrentPassword = "TestPassword!@2334";
        const string testingNewPassword = "TestNewPassword@@!1233";

        
        await _userBuilder
            .Build(userId: testingUserId, username: testingUsername, email: testingEmail,
                password: testingCurrentPassword, userRole: testingUserRole);
        
        UpdatePasswordCommand command = new UpdatePasswordCommand
            (testingCurrentPassword, testingNewPassword, testingNewPassword);
        
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
    public async Task UserUpdatePassword_WrongCurrentPassword_BaseErrorCodeShouldBe_UserUnauthorizedOperation()
    {
        //Arrange
        const int testingUserId = 56643;
        const string testingEmail = "test@gmail.com";
        const string testingUsername = "testUsername";
        const UserRole testingUserRole = UserRole.User;

        const string testingBadCurrentPassword = "TestPassword!@5555";
        const string testingRealCurrentPassword = "TestPassword!@2334";
        const string testingNewPassword = "TestNewPassword@@!1233";
        
        await _userBuilder
            .Build(userId: testingUserId, username: testingUsername, email: testingEmail,
                password: testingRealCurrentPassword, userRole: testingUserRole);

        UpdatePasswordCommand command = new UpdatePasswordCommand
            (testingBadCurrentPassword, testingNewPassword, testingNewPassword);
        
        //Act
        HttpResponseModule responseModule = 
            ActResponseModule(command, GetUserJwtToken(testingUserId));
        
        
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
    public async Task UserUpdatePassword_NewPasswordsAreNotEqual_BaseErrorCodeShouldBe_FluentValidationCode()
    {
        //Arrange
        const int testingUserId = 2560;
        const string testingEmail = "test@gmail.com";
        const string testingUsername = "testUsername";
        const UserRole testingUserRole = UserRole.User;
        
        const string testingCurrentPassword = "TestPassword!@2334";
        const string testingNewPassword = "TestPassword!@55523";
        const string testingNewConfirmPassword = "TestPassword!@3123";


        await _userBuilder
            .Build(userId: testingUserId, username: testingUsername, email: testingEmail,
            password: testingCurrentPassword, userRole: testingUserRole);

        UpdatePasswordCommand command = new UpdatePasswordCommand
            (testingCurrentPassword, testingNewPassword, testingNewConfirmPassword);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(command, GetUserJwtToken(testingUserId));

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        });
    }
    
    private HttpResponseModule ActResponseModule
        (UpdatePasswordCommand command, string jwtToken)
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