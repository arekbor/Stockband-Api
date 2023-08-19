using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.FeatureServices;
using Stockband.Application.Interfaces.FeatureServices;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Enums;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.IntegrationTests.FeatureServices;

[TestFixture]
public class RefreshTokenServiceTest:BaseTest
{
    private IRefreshTokenRepository _refreshTokenRepository = null!;
    private IRefreshTokenService _refreshTokenService = null!;
    private IUserRepository _userRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _refreshTokenRepository = new RefreshTokenRepository(Context);
        _refreshTokenService = new RefreshTokenService(_refreshTokenRepository);
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public async Task GetRefreshToken_ShouldReturnToken_And_PersistToDatabase()
    {
        //Arrange
        const int testingUserId = 125;
        const string testingCreatedIp = "127.0.0.1";
        const double testingTokenExpiresDays = 20;

        User user = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingUserId)
            .Build();
        await _userRepository.AddAsync(user);
        
        //Act
        BaseResponse<string> response = await _refreshTokenService
            .GetRefreshToken(testingUserId, testingCreatedIp, testingTokenExpiresDays);

        RefreshToken? refreshToken = await _refreshTokenRepository.GetRefreshTokenByToken(response.Result);
        if (refreshToken == null)
        {
            throw new ObjectNotFound(typeof(RefreshToken),response.Result);
        }

        //Assert
        response.Errors.Count.ShouldBe(0);
        refreshToken.Token.ShouldBe(response.Result);
        refreshToken.CreatedByIp.ShouldBe(testingCreatedIp);
        refreshToken.Expires.Date.ShouldBe(DateTime.Today.AddDays(testingTokenExpiresDays));
    }

    [Test]
    public async Task RevokeDescendantRefreshTokens_ShouldRevoked_LatestRefreshToken()
    {
        //Arrange
        const int testingUserId = 155;
        const string testingToken = "aaabbbDDDCC121333355";
        const string testingRevokedToken = "aaa";
        const string testingRevokedByIp = "127.0.0.1";

        User user = Builder<User>
            .CreateNew()
            .With(x => x.Id = testingUserId)
            .Build();

        await _userRepository.AddAsync(user);

        List<RefreshToken> refreshTokens = Builder<RefreshToken>
            .CreateListOfSize(5)
            .All()
            .With(x => x.Expires = DateTime.Now.AddDays(20))
            .With(x => x.CreatedByIp = "127.0.0.1")
            .With(x => x.CreatedAt = DateTime.Now)
            .With(x => x.Deleted = false)
            .With(x => x.UserId = testingUserId)
            .With(x => x.RevokedByIp = "127.0.0.1")
            .With(x => x.ReplacedByToken = null)
            .With(x => x.Revoked = DateTime.Now)
            .With(x => x.ReasonRevoke = RefreshTokenReasonRevoke.ReplacedByNewToken)
            
            .TheFirst(1)
            .With(x => x.Token = testingRevokedToken)
            .With(x => x.ReplacedByToken = "bbb")
            
            .TheNext(1)
            .With(x => x.Token = "bbb")
            .With(x => x.ReplacedByToken = "ccc")
            
            .TheNext(1)
            .With(x => x.Token = "ccc")
            .With(x => x.ReplacedByToken = "ddd")
            
            .TheNext(1)
            .With(x => x.Token = "ddd")
            .With(x => x.ReplacedByToken = testingToken)
            
            .TheNext(1)
            .With(x => x.Token = testingToken)
            .With(x => x.RevokedByIp = null)
            .With(x => x.ReasonRevoke = null)
            .With(x => x.Revoked = null)

            .Build()
            .ToList();

        await _refreshTokenRepository.AddRangeAsync(refreshTokens);
        
        //Act
        RefreshToken? testingRevokedRefreshToken = await _refreshTokenRepository
            .GetRefreshTokenByToken(testingRevokedToken);
        if (testingRevokedRefreshToken == null)
        {
            throw new ObjectNotFound(typeof(RefreshToken), testingRevokedToken);
        }
        
        await _refreshTokenService
            .RevokeDescendantRefreshTokens(
                testingRevokedRefreshToken, 
                testingRevokedByIp, 
                RefreshTokenReasonRevoke.AttemptedReuseRevokedToken);

        RefreshToken? testingActualLatestToken = await _refreshTokenRepository
            .GetRefreshTokenByToken(testingToken);
        if (testingActualLatestToken == null)
        {
            throw new ObjectNotFound(typeof(RefreshToken), testingToken);
        }

        //Assert
        testingActualLatestToken.RevokedByIp.ShouldBe(testingRevokedByIp);
        testingActualLatestToken.Revoked.ShouldNotBeNull();
        testingActualLatestToken.IsRevoked.ShouldBe(true);
        testingActualLatestToken.ReasonRevoke.ShouldBe(RefreshTokenReasonRevoke.AttemptedReuseRevokedToken);
        testingActualLatestToken.ReplacedByToken.ShouldBeNull();
    }

    [Test]
    public async Task RevokeRefreshToken_Should_RevokedRefreshToken()
    {
        //Arrange
        const string testingToken = "123dddffggh";
        const string testingIp = "127.0.0.1";
        const int testingUserId = 125;
        const RefreshTokenReasonRevoke testingReason = RefreshTokenReasonRevoke.Manual;
        
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Id = testingUserId)
            .Build();

        await _userRepository.AddAsync(user);

        RefreshToken refreshToken = Builder<RefreshToken>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Revoked = null)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.RevokedByIp = null)
            .With(x => x.Token = testingToken)
            .With(x => x.UserId = testingUserId)
            .Build();
        await _refreshTokenRepository.AddAsync(refreshToken);

        //Act
        await _refreshTokenService
            .RevokeRefreshToken(refreshToken, testingIp, testingReason);

        RefreshToken? testingRefreshToken = await _refreshTokenRepository.GetRefreshTokenByToken(testingToken);
        if (testingRefreshToken == null)
        {
            throw new ObjectNotFound(typeof(RefreshToken), testingToken);
        }
        
        //Assert
        testingRefreshToken.Revoked.ShouldNotBeNull();
        testingRefreshToken.IsRevoked.ShouldBe(true);
        testingRefreshToken.RevokedByIp.ShouldBe(testingIp);
        testingRefreshToken.ReasonRevoke.ShouldBe(testingReason);
    }

    [Test]
    public async Task RemoveOldRefreshTokens_Should_RemoveOldTokens()
    {
        //Arrange
        const int testingUserId = 125;
        const int countOfTokens = 10;
        const int countOfActualTokens = 1;
        
        User user = Builder<User>
            .CreateNew()
            .With(x => x.Id = testingUserId)
            .Build();
        await _userRepository.AddAsync(user);

        List<RefreshToken> refreshTokens = Builder<RefreshToken>
            .CreateListOfSize(countOfTokens)
            .All()
            .With(x => x.CreatedByIp = "127.0.0.1")
            .With(x => x.RevokedByIp = "127.0.0.1")
            .With(x => x.CreatedAt = DateTime.Now.AddDays(-20))
            .With(x => x.Expires = DateTime.Now.AddDays(-50))
            .With(x => x.Deleted = false)
            .With(x => x.UserId = testingUserId)
            .With(x => x.Revoked = DateTime.Now.AddDays(-20))
            .With(x => x.ReasonRevoke = RefreshTokenReasonRevoke.ReplacedByNewToken)
            .TheLast(countOfActualTokens)
            .With(x => x.CreatedAt = DateTime.Now)
            .With(x => x.Revoked = null)
            .With(x => x.ReplacedByToken = null)
            .With(x => x.RevokedByIp = null)
            .With(x => x.ReasonRevoke = null)
            .With(x => x.Expires = DateTime.Now.AddDays(50))
            .Build()
            .ToList();
        await _refreshTokenRepository.AddRangeAsync(refreshTokens);

        //Act
        await _refreshTokenService.RemoveOldRefreshTokens(testingUserId, 10);

        List<RefreshToken> testingRefreshTokens = await _refreshTokenRepository
            .GetAllUserRefreshTokens(testingUserId);
        
        //Assert
        testingRefreshTokens.Count.ShouldBe(countOfActualTokens);
    }
}