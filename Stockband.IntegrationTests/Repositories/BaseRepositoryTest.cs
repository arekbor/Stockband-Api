using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.IntegrationTests.Repositories;

[TestFixture]
public class BaseRepositoryTest:BaseTest
{
    private IBaseRepository<User> _baseRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _baseRepository = new BaseRepository<User>(Context);
    }

    [Test]
    public async Task GetByIdAsync_ShouldReturnObject()
    {
        //Arrange
        const int testingUserId = 123;

        List<User> usersForTest =  Builder<User>
            .CreateListOfSize(100)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Id = testingUserId)
            .Build()
            .ToList();
        await _baseRepository.AddRangeAsync(usersForTest);
        
        //Act
        User? result = await _baseRepository.GetByIdAsync(testingUserId);

        //Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(testingUserId);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllObjects()
    {
        //Arrange
        List<User> usersForTest = Builder<User>
            .CreateListOfSize(100)
            .All()
            .With(x => x.Deleted = false)
            .Build()
            .ToList();
        await _baseRepository.AddRangeAsync(usersForTest);
        
        //Act
        IEnumerable<User> result = await _baseRepository.GetAllAsync();

        //Assert
        result.Count().ShouldBe(usersForTest.Count);
    }

    [Test]
    public async Task AddAsync_ObjectShouldBeSuccessfullyCreated()
    {
        //Arrange
        const int testingUserId = 500;
        string testingUsername = new Faker().Person.UserName;
        string testingUserEmail = new Faker().Person.Email;
        const UserRole testingUserRole = UserRole.User;
        
        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Id = testingUserId)
            .With(x => x.Email = testingUserEmail)
            .With(x => x.Role = testingUserRole)
            .With(x => x.Username = testingUsername)
            .Build();

        //Act
        await _baseRepository.AddAsync(userForTest);
        User? testingUser = await _baseRepository.GetByIdAsync(testingUserId);

        //Assert
        testingUser.ShouldNotBeNull();
        testingUser.Id.ShouldBe(testingUserId);
        testingUser.Email.ShouldBe(testingUserEmail);
        testingUser.Username.ShouldBe(testingUsername);
        testingUser.Role.ShouldBe(testingUserRole);
    }

    [Test]
    public async Task AddRangeAsync_AllObjectsShouldBeSuccessfullyCreated()
    {
        //Arrange
        const int firstTestingUserId = 1250;
        string firstTestingUsername = new Faker().Person.UserName;
        string firstTestingEmail = new Faker().Person.Email;
        UserRole firstTestingUserRole = UserRole.User;
        
        const int secondTestingUserId = 5240;
        string secondTestingUsername = new Faker().Person.UserName;
        string secondTestingEmail = new Faker().Person.Email;
        UserRole secondTestingUserRole = UserRole.User;

        List<User> testingUsers = Builder<User>
            .CreateListOfSize(2)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Id = firstTestingUserId)
            .With(x => x.Username = firstTestingUsername)
            .With(x => x.Email = firstTestingEmail)
            .With(x => x.Role = firstTestingUserRole)
            .TheLast(1)
            .With(x => x.Id = secondTestingUserId)
            .With(x => x.Username = secondTestingUsername)
            .With(x => x.Email = secondTestingEmail)
            .With(x => x.Role = secondTestingUserRole)
            .Build()
            .ToList();
        
        //Act
        await _baseRepository.AddRangeAsync(testingUsers);
        IEnumerable<User> result = await _baseRepository.GetAllAsync();

        //Assert
        result.Count().ShouldBe(testingUsers.Count);
        
        result.First().Id.ShouldBe(firstTestingUserId);
        result.First().Username.ShouldBe(firstTestingUsername);
        result.First().Email.ShouldBe(firstTestingEmail);
        result.First().Role.ShouldBe(firstTestingUserRole);
        
        result.Last().Id.ShouldBe(secondTestingUserId);
        result.Last().Username.ShouldBe(secondTestingUsername);
        result.Last().Email.ShouldBe(secondTestingEmail);
        result.Last().Role.ShouldBe(secondTestingUserRole);
    }
    
    [Test]
    public async Task UpdateAsync_ObjectShouldBeUpdated()
    {
        //Arrange
        const int testingUserId = 54260;
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        UserRole testingUserRole = UserRole.User;
        
        string testingUsernameUpdate = new Faker().Person.UserName;
        string testingEmailUpdate = new Faker().Person.Email;
        UserRole testingUserRoleUpdate = UserRole.Admin;

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingUserId)
            .With(x => x.Username = testingUsername)
            .With(x => x.Email = testingEmail)
            .With(x => x.Role = testingUserRole)
            .Build();
        
        await _baseRepository.AddAsync(userForTest);
        
        //Act
        User? testingUserToUpdate = await _baseRepository.GetByIdAsync(testingUserId);
        if (testingUserToUpdate == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }

        testingUserToUpdate.Username = testingUsernameUpdate;
        testingUserToUpdate.Email = testingEmailUpdate;
        testingUserToUpdate.Role = testingUserRoleUpdate;
        await _baseRepository.UpdateAsync(testingUserToUpdate);

        User? updatedUserTest = await _baseRepository.GetByIdAsync(testingUserId);
        if (updatedUserTest == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }

        //Assert
        updatedUserTest.Username.ShouldNotBe(testingUsername);
        updatedUserTest.Email.ShouldNotBe(testingEmail);
        updatedUserTest.Role.ShouldNotBe(testingUserRole);
    }

    [Test]
    public async Task DeleteAsync_ObjectShouldBeDeleted()
    {
        //Arrange
        const int testingUserId = 223441;
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        UserRole testingUserRole = UserRole.User;
        
        List<User> usersForTest = Builder<User>
            .CreateListOfSize(10)
            .All()
            .With(x => x.Deleted = false)
            .TheFirst(1)
            .With(x => x.Id = testingUserId)
            .With(x => x.Username = testingUsername)
            .With(x => x.Email = testingEmail)
            .With(x => x.Role = testingUserRole)
            .Build()
            .ToList();
        await _baseRepository.AddRangeAsync(usersForTest);

        //Act
        User? userForTest = await _baseRepository.GetByIdAsync(testingUserId);
        if (userForTest == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }
        
        await _baseRepository.DeleteAsync(userForTest);
        IEnumerable<User> result = await _baseRepository.GetAllAsync();

        //Assert
        result.Count().ShouldBe(usersForTest.Count-1);
    }
}