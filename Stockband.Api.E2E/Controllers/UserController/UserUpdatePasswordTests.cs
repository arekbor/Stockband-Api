using System.Net;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.Dtos.User;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class UserUpdatePasswordTests:BaseTest
{
    private IUserRepository _userRepository = null!;
    private const string TestingUri = "/user/password";

    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
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

        await UserBuilder
            (testingCurrentPassword, testingUsername, testingEmail, testingUserRole, testingUserId);

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
            (testingCurrentPassword, testingNewPassword, testingNewPassword);
        
        //Act
        HttpResponseModule responseModule = 
            ActResponseModule(dto, testingUserId, testingUsername, testingEmail, testingUserRole);
        
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
        
        await UserBuilder
            (testingRealCurrentPassword, testingUsername, testingEmail, testingUserRole, testingUserId);

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
            (testingBadCurrentPassword, testingNewPassword, testingNewPassword);
        
        //Act
        HttpResponseModule responseModule = 
            ActResponseModule(dto, testingUserId, testingUsername, testingEmail, testingUserRole);
        
        
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
        
        await UserBuilder
            (testingCurrentPassword, testingUsername, testingEmail, testingUserRole, testingUserId);

        UpdateUserPasswordDto dto = new UpdateUserPasswordDto
            (testingCurrentPassword, testingNewPassword, testingNewConfirmPassword);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, testingUserId, testingUsername, testingEmail, testingUserRole);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        });
    }
    
    private async Task UserBuilder
        (string currentPassword, string username, string email, UserRole userRole, int id)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(currentPassword);
        
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Username = username)
            .With(x => x.Email = email)
            .With(x => x.Role = userRole)
            .With(x => x.Password = hash)
            .With(x => x.Id = id)
            .Build();
        await _userRepository.AddAsync(user);
    }

    private HttpResponseModule ActResponseModule
        (UpdateUserPasswordDto dto, int jwtUserId, string jwtUserUsername, string jwtUserEmail, UserRole jwtRole)
    {
        return HttpHost
            .Put
            .Url(TestingUri)
            .WithJwtToken(GetJwtToken(jwtUserId, jwtUserUsername, jwtUserEmail, jwtRole))
            .Json(dto)
            .Send()
            .Response;
    }
}