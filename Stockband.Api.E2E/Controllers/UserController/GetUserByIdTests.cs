using System.Net;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class GetUserByIdTests:BaseTest
{
    private static Func<int, string> TestingUri = id => $"/user/{id}";
    
    private IUserRepository _userRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public async Task GetUserById_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 2425;

        await UserBuilder(testingUserId);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(testingUserId, UserRole.Admin);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(0);
            response.Success.ShouldBe(true);
        });
    }

    [Test]
    public async Task GetUserById_UserNotFound_BaseErrorCodeShouldBe_UserNotExists()
    {
        //Arrange
        const int testingUserId = 5663;
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(testingUserId, UserRole.Admin);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotExists);
        });
    }

    [Test]
    public void GetUserById_RequestedUserIsNotAdmin_HttpStatusCodeShouldBe_Forbidden()
    {
        //Arrange
        const int testingUserId = 5200;
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(testingUserId, UserRole.User);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    private HttpResponseModule ActResponseModule(int userId, UserRole executingUserRole)
    {
        string url = TestingUri(userId);
        
        return HttpHost
            .Get
            .Url(url)
            .WithJwtToken(GetJwtToken(500, "admin", "admin@stockband.com", executingUserRole))
            .Send()
            .Response;
    }

    private async Task UserBuilder(int userId)
    {
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = userId)
            .With(x => x.Role = UserRole.User)
            .Build();

        await _userRepository.AddAsync(user);
    }
}