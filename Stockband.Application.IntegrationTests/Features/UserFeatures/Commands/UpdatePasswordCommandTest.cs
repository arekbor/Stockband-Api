using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.UserFeatures.Commands;

public class UpdatePasswordCommandTest:BaseTest
{
    [Test]
    public async Task UpdatePasswordCommand_ResponseShouldBeSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 105;
        const string testingCurrentPassword = "usermocktestpassword1";
        const string testingNewPassword = "TesTPassWord!@23";
        
        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingNewPassword
        };
        
        string? hashedTestingCurrentPassword = BCrypt.Net.BCrypt.HashPassword(testingCurrentPassword);
        if (hashedTestingCurrentPassword == null)
        {
            throw new ObjectNotFound(nameof(hashedTestingCurrentPassword));
        }
        
        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Id = testingRequestedUserId)
            .With(x => x.Password = hashedTestingCurrentPassword)
            .Build();
        await userRepository.AddAsync(userForTest);

        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        User? userAfterHandler = await userRepository.GetByIdAsync(testingRequestedUserId);
        if (userAfterHandler == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }

        bool verifyNewPassword = BCrypt.Net.BCrypt.Verify(testingNewPassword, userAfterHandler.Password);

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        verifyNewPassword.ShouldBe(true);
    }

    [Test]
    public async Task UpdatePasswordCommand_PasswordsNotEquals_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 322131234;
        const string testingCurrentPassword = "usermocktestpassword1";
        const string testingNewPassword = "TesTPassWord!@23";
        const string testingConfirmPassword = "TesTPassWord!@23abcd";
        
        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingConfirmPassword
        };
        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }

    [Test]
    public async Task UpdatePasswordCommand_InvalidCurrentPassword_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 33213;
        const string testingCurrentPassword = "usermocktestpassword1";
        const string testingNewPassword = "TesTPassWord!@23";
        
        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingNewPassword
        };
        
        string? hashedTestingCurrentPassword = BCrypt.Net.BCrypt.HashPassword(testingCurrentPassword);
        if (hashedTestingCurrentPassword == null)
        {
            throw new ObjectNotFound(nameof(hashedTestingCurrentPassword));
        }
        
        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Password = "$2a$11$o.e5SfIYGqby.NVyq/6vrONg76hL6yo2R2DZkuH1esekWcd6ihQnO")
            .With(x => x.Id = testingRequestedUserId)
            .Build();
        await userRepository.AddAsync(userForTest);

        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }

    [Test]
    public async Task UpdatePasswordCommand_UserNotExists_ResponseShouldBeNotSuccess()
    {
        //Arrange
        UserRepository userRepository = new UserRepository(Context);
        
        const int testingRequestedUserId = 120332213;
        const string testingCurrentPassword = "usermocktestpassword1";
        const string testingNewPassword = "TesTPassWord!@23";

        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingNewPassword
        };
        
        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotExists);
    }
}