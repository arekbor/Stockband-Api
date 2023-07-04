using System.Net;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class GetUserByIdTests:BaseTest
{
    private static readonly Func<int, string> TestingUri = id => $"/user/{id}";
    
    private UserBuilder _userBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
    }

    [Test]
    public async Task GetUserById_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const int testingUserId = 2425;

        await _userBuilder
            .Build(userId: testingUserId);
        
        //Act
        HttpResponseModule responseModule = 
            ActResponseModule(testingUserId, GetAdminJwtToken(3244));
        
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
        HttpResponseModule responseModule = 
            ActResponseModule(testingUserId, GetAdminJwtToken(5400));
        
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
        HttpResponseModule responseModule = 
            ActResponseModule(testingUserId, GetUserJwtToken(5000));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.Forbidden);
    }

    private HttpResponseModule ActResponseModule(int userId, string jwtToken)
    {
        string url = TestingUri(userId);
        
        return HttpHost
            .Get
            .Url(url)
            .WithJwtToken(jwtToken)
            .Send()
            .Response;
    }
}