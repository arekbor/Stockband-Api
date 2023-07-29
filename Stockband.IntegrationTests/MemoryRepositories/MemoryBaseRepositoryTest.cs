using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.MemoryRepositories;
using Stockband.Domain.Exceptions;
using Stockband.Domain.MemoryEntities;
using Stockband.Infrastructure.MemoryRepositories;

namespace Stockband.IntegrationTests.MemoryRepositories;

[TestFixture]
public class MemoryBaseRepositoryTest:BaseTest
{
    private IMemoryBaseRepository<UserRefreshToken> _memoryBaseRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _memoryBaseRepository = new MemoryBaseRepository<UserRefreshToken>(StockbandMemoryDbContext);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnObject()
    {
        //Arrange
        const int testingUserRefreshTokenId = 500;
        List<UserRefreshToken> userRefreshTokens = Builder<UserRefreshToken>
            .CreateListOfSize(100)
            .TheFirst(1)
            .With(x => x.Id = testingUserRefreshTokenId)
            .Build()
            .ToList();
        await _memoryBaseRepository.AddRangeAsync(userRefreshTokens);

        //Act
        UserRefreshToken? result = await _memoryBaseRepository.GetByIdAsync(testingUserRefreshTokenId);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(testingUserRefreshTokenId);
    }

    [Test]
    public async Task AddAsync_ObjectShouldBeSuccessfullyCreated()
    {
        //Arrange
        const int userRefreshTokenTestId = 525;
        UserRefreshToken userRefreshToken = Builder<UserRefreshToken>
            .CreateNew()
            .With(x => x.Id = userRefreshTokenTestId)
            .Build();
        //Act
        await _memoryBaseRepository.AddAsync(userRefreshToken);

        UserRefreshToken? result = await _memoryBaseRepository.GetByIdAsync(userRefreshTokenTestId);
        if (result == null)
        {
            throw new ObjectNotFound(typeof(UserRefreshToken), userRefreshTokenTestId);
        }

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userRefreshTokenTestId);
    }

    [Test]
    public async Task UpdateAsync_ObjectShouldBeUpdated()
    {
        //Arrange
        const int userRefreshTokenId = 125;
        const string testingToken = "test-token";
        UserRefreshToken userRefreshToken = Builder<UserRefreshToken>
            .CreateNew()
            .With(x => x.Id = userRefreshTokenId)
            .With(x => x.Token = testingToken)
            .Build();
        await _memoryBaseRepository.AddAsync(userRefreshToken);

        //Act
        userRefreshToken.Token = "super-test-token";
        await _memoryBaseRepository.UpdateAsync(userRefreshToken);

        UserRefreshToken? result = await _memoryBaseRepository.GetByIdAsync(userRefreshTokenId);
        if (result == null)
        {
            throw new ObjectNotFound(typeof(UserRefreshToken), userRefreshTokenId);
        }
        
        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userRefreshTokenId);
        result.Token.ShouldNotBe(testingToken);
    }

    [Test]
    public async Task DeleteAsync_ObjectShouldBeDeleted()
    {
        //Arrange
        const int userRefreshTokenTestId = 525;
        UserRefreshToken userRefreshToken = Builder<UserRefreshToken>
            .CreateNew()
            .With(x => x.Id = userRefreshTokenTestId)
            .Build();
        await _memoryBaseRepository.AddAsync(userRefreshToken);

        //Act
        await _memoryBaseRepository.DeleteAsync(userRefreshToken);
        UserRefreshToken? result = await _memoryBaseRepository.GetByIdAsync(userRefreshTokenTestId);
        
        //Assert
        result.ShouldBeNull();
    }
}