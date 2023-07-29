using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.IntegrationTests.Repositories;

[TestFixture]
public class UserRepositoryTest:BaseTest
{
    private IUserRepository _userRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(StockbandDbContext);
    }

    [Test]
    public async Task GetUserByEmailAsync_ShouldReturnObject()
    {
        //Arrange
        string testingUserEmail = new Faker().Person.Email;
        const int testingUserId = 4220;

        List<User> userForTest = Builder<User>
            .CreateListOfSize(5)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Email = testingUserEmail)
            .With(x => x.Id = testingUserId)
            .Build()
            .ToList();
        await _userRepository.AddRangeAsync(userForTest);

        //Act
        User? result = await _userRepository.GetUserByEmailAsync(testingUserEmail);
        if (result == null)
        {
            throw new ObjectNotFound(typeof(User));
        }

        //Assert
        result.Id.ShouldBe(testingUserId);
        result.Email.ShouldBe(testingUserEmail);
    }

    [Test]
    public async Task GetUserByUsernameAsync_ShouldReturnObject()
    {
        //Arrange
        string testingUserUsername = new Faker().Person.Email;
        const int testingUserId = 4220;

        List<User> userForTest = Builder<User>
            .CreateListOfSize(5)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Username = testingUserUsername)
            .With(x => x.Id = testingUserId)
            .Build()
            .ToList();
        await _userRepository.AddRangeAsync(userForTest);

        //Act
        User? result = await _userRepository.GetUserByUsernameAsync(testingUserUsername);
        if (result == null)
        {
            throw new ObjectNotFound(typeof(User));
        }

        //Assert
        result.Id.ShouldBe(testingUserId);
        result.Username.ShouldBe(testingUserUsername);
    }
}