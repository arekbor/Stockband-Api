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
public class UserUpdateTests:BaseTest
{
    private IUserRepository _userRepository = null!;
    private const string TestingUri = "/user/update";

    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public async Task UserUpdate_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 54436;
        const string testingUpdateUsername = "testUsername";
        const string testingUpdateEmail = "test@gmail.com";
        
        await UserBuilder(testingUserId);
        
        UpdateUserDto dto = new UpdateUserDto(testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, testingUserId, "testUsername", "user@test.com", UserRole.User);

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

        await UserBuilder(testingRequestedId, testingRequestedUsername, testingRequestedEmail);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, testingRequestedId, testingRequestedUsername, testingRequestedEmail, UserRole.User);
        
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

        await UserBuilder(testingRequestedId, testingRequestedUsername, testingRequestedEmail, UserRole.Admin);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, testingRequestedId, testingRequestedUsername, testingRequestedEmail, UserRole.Admin);
        
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
    [TestCase(null, "existing@gmail.com")]
    [TestCase("existingUsername", null)]
    public async Task UserUpdate_ProvidedUsernameOrEmailAlreadyExists_BaseErrorCodeShouldBe_UserAlreadyCreated
        (string? username, string? email)
    {
        //Arrange
        const int testingUserId = 1200;
        string testingUpdateUsername = username ?? "updateUsername";
        string testingUpdateEmail = email ?? "update@gmail.com";
        UpdateUserDto dto = new UpdateUserDto(testingUserId, testingUpdateUsername, testingUpdateEmail);
        
        const string testingRequestedUsername = "requestedUser";
        const string testingRequestedEmail = "requested@gmail.com";
        await UserBuilder(testingUserId, testingRequestedUsername, testingRequestedEmail);

        const int testingExistingUserId = 5430;
        string testingExistingUsername = username ?? "existingUsername";
        string testingExistingEmail = email ?? "existing@gmail.com";
        await UserBuilder(testingExistingUserId, testingExistingUsername, testingExistingEmail);

        //Act
        HttpResponseModule responseModule = ActResponseModule
            (dto, testingUserId, testingRequestedUsername, testingRequestedEmail, UserRole.User);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(false);
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserAlreadyCreated);
        });
    }

    private HttpResponseModule ActResponseModule
        (UpdateUserDto dto, int userId, string username, string email, UserRole userRole)
    {
        return HttpHost
            .Put
            .Url(TestingUri)
            .WithJwtToken(GetJwtToken(userId, username, email, userRole))
            .Json(dto)
            .Send()
            .Response;
    }

    private async Task UserBuilder(int userId, string? username = "", string? email = "", UserRole userRole = UserRole.User)
    {
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted)
            .With(x => x.Id = userId)
            .With(x => x.Username = username?? "test")
            .With(x => x.Email = email?? "testing@gmail.com")
            .With(x => x.Role = userRole)
            .Build();
        await _userRepository.AddAsync(user);
    }
}