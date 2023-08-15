using System.Net;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.E2E.Builders;
using Stockband.Application.Features.UserFeatures.Queries.RefreshToken;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class RefreshTokenTests:BaseTest
{
    private const string TestingUrl = "/user/refresh";
    private UserBuilder _userBuilder = null!;
    private IUserRepository _userRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _userBuilder = new UserBuilder(Context);
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public async Task RefreshToken_RevokedToken_BaseErrorCodeShouldBe_InvalidRefreshToken()
    {
        //Arrange
        const int testingUserId = 1255;
        const string testingRefreshToken = "123456789";
        const string testingRevokedToken = "aaaddssdbvb123123";

        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(2)
            .All()
            .With(x => x.Revoked = null)
            .With(x => x.ReasonRevoke = null)
            .With(x => x.CreatedByIp = "127.0.0.1")
            .With(x => x.ReplacedByToken = null)
            .With(x => x.Deleted = false)
            .With(x => x.RevokedByIp = null)
            .TheFirst(1)
            .With(x => x.ReplacedByToken = testingRefreshToken)
            .With(x => x.Token = testingRevokedToken)
            .With(x => x.Revoked = DateTime.Now.AddDays(-1))
            .With(x => x.ReasonRevoke = RefreshTokenReasonRevoke.ReplacedByNewToken)
            .TheLast(1)
            .With(x => x.Token = testingRefreshToken)
            .With(x => x.ReplacedByToken = null)
            .Build()
            .ToList();
        
        await _userBuilder.Build(userId: testingUserId, refreshTokens: userRefreshTokens);
        
        RefreshTokenQuery query = new RefreshTokenQuery
        {
            RefreshToken = testingRevokedToken
        };
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(query);
        
        User? testingUser = await _userRepository.GetUserByRefreshToken(testingRevokedToken);
        if (testingUser == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }

        UserRefreshToken testingLatestRefreshToken =
            testingUser.UserRefreshTokens.First(x => x.Token == testingRefreshToken);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.InvalidRefreshToken);
        });
        testingLatestRefreshToken.Revoked.ShouldNotBeNull();
        testingLatestRefreshToken.IsRevoked.ShouldBe(true);
    }

    [Test]
    public async Task RefreshToken_ExpiredToken_BaseErrorCodeShouldBe_InvalidRefreshToken()
    {
        //Arrange
        const string testingRefreshToken = "123456789";
        
        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(1)
            .All()
            .With(x => x.Revoked = null)
            .With(x => x.ReasonRevoke = null)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.CreatedByIp = "127.0.0.1")
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Token = testingRefreshToken)
            .With(x => x.Expires = DateTime.Now.AddDays(-30))
            .With(x => x.CreatedAt = DateTime.Now)
            .Build()
            .ToList();
        
        await _userBuilder.Build(userId: 125, refreshTokens: userRefreshTokens);
        
        RefreshTokenQuery query = new RefreshTokenQuery
        {
            RefreshToken = testingRefreshToken
        };
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(query);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.InvalidRefreshToken);
        });
    }

    [Test]
    public async Task RefreshToken_AfterGeneratedNewRefreshToken_AllOldRefreshToken_ShouldBe_Deleted()
    {
        //Arrange
        const string testingRefreshToken = "555555";
        const int testingUserId = 550;
        const int countOfAllTokens = 6;
        
        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(countOfAllTokens)
            .All()
            .With(x => x.Revoked = null)
            .With(x => x.ReasonRevoke = null)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.CreatedByIp = "127.0.0.1")
            .With(x => x.Deleted = false)
            .TheFirst(countOfAllTokens-1)
            .With(x => x.Token = "awdawdawdawdrtargsdfrv234fr")
            .With(x => x.Expires = DateTime.Now.AddDays(-15))
            .With(x => x.CreatedAt = DateTime.Now.AddDays(-15))
            .TheLast(1)
            .With(x => x.Token = testingRefreshToken)
            .With(x => x.Expires = DateTime.Now.AddDays(7))
            .With(x => x.CreatedAt = DateTime.Now)
            .Build()
            .ToList();
        
        await _userBuilder.Build(userId: testingUserId, refreshTokens:userRefreshTokens);

        RefreshTokenQuery query = new RefreshTokenQuery
        {
            RefreshToken = testingRefreshToken
        };
        
        //Act
        ActResponseModule(query);
        
        User? userResponse = await _userRepository.GetUserByRefreshToken(testingRefreshToken);
        if (userResponse == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }

        //Assert
        userResponse.UserRefreshTokens.Count.ShouldBe(2);
    }

    [Test]
    public async Task RefreshToken_LatestToken_ShouldBe_SameAs_PreviousToken_ReplacedByToken()
    {
        //Arrange
        const string testingRefreshToken = "123456789";
        const int testingUserId = 125;

        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(1)
            .All()
            .With(x => x.Revoked = null)
            .With(x => x.ReasonRevoke = null)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.CreatedByIp = "127.0.0.1")
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Token = testingRefreshToken)
            .With(x => x.Expires = DateTime.Now.AddDays(7))
            .With(x => x.CreatedAt = DateTime.Now)
            .Build()
            .ToList();

        await _userBuilder.Build(userId: testingUserId, refreshTokens: userRefreshTokens);
        
        RefreshTokenQuery query = new RefreshTokenQuery
        {
            RefreshToken = testingRefreshToken
        };
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(query);
        
        User? userResponse = await _userRepository.GetUserByRefreshToken(testingRefreshToken);
        if (userResponse == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }
        UserRefreshToken userRefreshTokenResult = userResponse
            .UserRefreshTokens.Single(x => x.Token == testingRefreshToken);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);
        responseModule.AsJson.AssertThat<BaseResponse<RefreshTokenQueryViewModel>>(response =>
        {
            response.Result.RefreshToken.ShouldBe(userRefreshTokenResult.ReplacedByToken);
            userRefreshTokenResult.Revoked.ShouldNotBeNull();
            userRefreshTokenResult.ReasonRevoke.ShouldBe(RefreshTokenReasonRevoke.ReplacedByNewToken);
            userRefreshTokenResult.RevokedByIp.ShouldNotBeEmpty();
        });
    }
    
    private HttpResponseModule ActResponseModule(RefreshTokenQuery query)
    {
        return HttpHost
            .Post
            .Url(TestingUrl)
            .Json(query)
            .Send()
            .Response;
    }
}