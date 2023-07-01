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
public class UserRoleTests:BaseTest
{
    private IUserRepository _userRepository = null!;
    private const string TestingUri = "/user/role";

    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public async Task UserRole_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 45530;
        const UserRole testingUserRole = UserRole.User;
        await UserBuilder(testingUserId, UserRole.User);

        const int testingRequestedUserId = 43350;
        await UserBuilder(testingRequestedUserId, UserRole.Admin);
        
        UpdateRoleDto dto = new UpdateRoleDto(testingUserId, testingUserRole);

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, testingRequestedUserId);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Success.ShouldBe(true);
            response.Errors.Count.ShouldBe(0);
        });
    }

    [Test]
    public async Task UserRole_OutsideOfTheUserRoleEnum_BaseErrorCodeShouldBe_FluentValidationCode()
    {
        //Arrange
        const int testingUserId = 45530;
        const UserRole testingUserRole = (UserRole)9000;
        
        UpdateRoleDto dto = new UpdateRoleDto(testingUserId, testingUserRole);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, 2933);
        
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
    public void UserRole_RequestedUserIsNotAdmin_HttpStatusCodeShouldBe_Forbidden()
    {
        //Arrange
        const int testingUserId = 3224;
        const UserRole testingUserRole = UserRole.Admin;
        
        UpdateRoleDto dto = new UpdateRoleDto(testingUserId, testingUserRole);

        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, 2933, "user", "user@gmail.com", UserRole.User);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    [Test]
    public async Task UserRole_ProvidedUserNotExists_BaseErrorCodeShouldBe_UserNotExists()
    {
        //Arrange
        const int testingUserId = 5127;
        const UserRole testingUserRole = UserRole.User;

        const int testingRequestedUserId = 5400;
        await UserBuilder(testingRequestedUserId, UserRole.Admin);
        
        UpdateRoleDto dto = new UpdateRoleDto(testingUserId, testingUserRole);
        
        //Act
        HttpResponseModule responseModule =
            ActResponseModule(dto, testingRequestedUserId);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotExists);
        });
    }

    private HttpResponseModule ActResponseModule
        (UpdateRoleDto dto, int userId, string username = "admin", string email = "admin@gmail.com", UserRole userRole = UserRole.Admin)
    {
        return HttpHost
            .Put
            .Url(TestingUri)
            .WithJwtToken(GetJwtToken(userId, username, email, userRole))
            .Json(dto)
            .Send()
            .Response;
    }

    private async Task UserBuilder(int userId, UserRole userRole)
    {
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = userRole)
            .With(x => x.Id = userId)
            .Build();
        await _userRepository.AddAsync(user);
    }
}