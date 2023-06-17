using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.UpdateUser;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.UserFeatures.Commands;

public class UpdateUserCommandTest:BaseTest
{
    [Test]
    public async Task UpdateUserCommand_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 2006;
        const int testingUserId = 2006;
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        
        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail
        };

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userForTest);
        
        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        User? updatedUser = await userRepository.GetByIdAsync(testingUserId);
        if (updatedUser == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        updatedUser.Email.ShouldBe(testingEmail);
        updatedUser.Username.ShouldBe(testingUsername);
        updatedUser.Id.ShouldBe(testingUserId);
    }

    [Test]
    public async Task UpdateUserCommand_RequestedUserIsNotOwnerButIsAdmin_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 20012;
        const int testingUserId = 76341;
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        
        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail,
        };

        List<User> usersForTest = Builder<User>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.Admin)
            .With(x => x.Id = testingRequestedUserId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingUserId)
            .Build()
            .ToList();
        await userRepository.AddRangeAsync(usersForTest);

        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        User? updatedUser = await userRepository.GetByIdAsync(testingUserId);
        if (updatedUser == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }
        
        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        updatedUser.Email.ShouldBe(testingEmail);
        updatedUser.Username.ShouldBe(testingUsername);
        updatedUser.Id.ShouldBe(testingUserId);
    }

    [Test]
    public async Task UpdateUserCommand_InvalidRequestedUser_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 50012;
        const int testingUserId = 71121;
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        
        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail
        };
        
        List<User> usersForTest = Builder<User>
            .CreateListOfSize(3)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingRequestedUserId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.User)
            .With(x => x.Id = testingUserId)
            .Build()
            .ToList();
        await userRepository.AddRangeAsync(usersForTest);
        
        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }

    [Test]
    [TestCaseSource(typeof(UserFeaturesTestDataCases), nameof(UserFeaturesTestDataCases.InvalidUsernameOrEmailCases))]
    public async Task UpdateUserCommand_InvalidUsernameOrEmailCases_ResponseShouldBeNotSuccess
        (string testingUsername, string testingEmail)
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 50012;
        const int testingUserId = 71121;
        
        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail,
        };
        
        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}