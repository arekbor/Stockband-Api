using System.Net;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.UserFeatures.Commands.RevokeToken;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class RevokeRefreshTokenTests:BaseTest
{
    private const string TestingUrl = "/user/revoke";
    private UserBuilder _userBuilder = null!;

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
    }
    
    [Test]
    public async Task RevokeRefreshToken_NotActive_BaseErrorCodeShouldBe_InvalidRefreshToken()
    {
        //Arrange
        const string testingRevokeToken = "123311asd";
        const int testingUserId = 156;

        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(1)
            .All()
            .With(x => x.Deleted = false)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.Revoked = null)
            .With(x => x.CreatedByIp = "127.0.0.1")
            .TheFirst(1)
            .With(x => x.Token = testingRevokeToken)
            .With(x => x.Expires = DateTime.Now.AddDays(-50))
            .With(x => x.CreatedAt = DateTime.Now)
            .Build()
            .ToList();

        await _userBuilder.Build(userId: testingUserId, refreshTokens: userRefreshTokens);

        RevokeTokenCommand command = new RevokeTokenCommand
        {
            RefreshToken = testingRevokeToken
        };
        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingUserId));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.InvalidRefreshToken);
            response.Success.ShouldBe(false);
        });
    }

    [Test]
    public async Task RevokeRefreshToken_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        const string testingRevokeToken = "123311asd";
        const int testingUserId = 156;

        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(1)
            .All()
            .With(x => x.Deleted = false)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.Revoked = null)
            .With(x => x.CreatedByIp = "127.0.0.1")
            .TheFirst(1)
            .With(x => x.Token = testingRevokeToken)
            .With(x => x.Expires = DateTime.Now.AddDays(12))
            .With(x => x.CreatedAt = DateTime.Now)
            .Build()
            .ToList();

        await _userBuilder.Build(userId: testingUserId, refreshTokens: userRefreshTokens);

        RevokeTokenCommand command = new RevokeTokenCommand
        {
            RefreshToken = testingRevokeToken
        };
        //Act
        HttpResponseModule responseModule = ActResponseModule(command, GetUserJwtToken(testingUserId));
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(0);
            response.Success.ShouldBe(true);
        });
    }
    
    private HttpResponseModule ActResponseModule(RevokeTokenCommand command, string jwtToken)
    {
        return HttpHost
            .Post
            .Url(TestingUrl)
            .WithJwtToken(jwtToken)
            .Json(command)
            .Send()
            .Response;
    }
}