using Moq;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.UpdatePassword;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.UserFeatures.Commands;

public class UpdatePasswordCommandTest
{
    private readonly Mock<IUserRepository> _mock;

    public UpdatePasswordCommandTest()
    {
        _mock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task UpdatePasswordCommand_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const string testingCurrentPassword = "usermocktestpassword1";
        const string testingNewPassword = "supertestpassword!@32WW";

        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(_mock.Object);

        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingNewPassword
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        User? user = _mock.Object.GetByIdAsync(testingRequestedUserId).Result;
        if (user == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }
        
        bool verify = BCrypt.Net.BCrypt.Verify(testingNewPassword, user.Password);
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        verify.ShouldBe(true);
    }
    
    [Fact]
    public async Task UpdatePasswordCommand_PasswordNotEquals_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const string testingCurrentPassword = "usermocktestpassword1";
        const string testingNewPassword = "supertestpassword!@32WW";
        const string testingConfirmNewPassword = "supertestpassword!@32WWsssss";

        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(_mock.Object);

        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingConfirmNewPassword
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        User? user = _mock.Object.GetByIdAsync(testingRequestedUserId).Result;
        if (user == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }
        
        bool verify = BCrypt.Net.BCrypt.Verify(testingNewPassword, user.Password);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        verify.ShouldBe(false);
    }
    
    [Fact]
    public async Task UpdatePasswordCommand_InvalidCurrentPassword_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const string testingCurrentPassword = "usermocktest";
        const string testingNewPassword = "supertestpassword!@32WW";

        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(_mock.Object);

        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingNewPassword
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        User? user = _mock.Object.GetByIdAsync(testingRequestedUserId).Result;
        if (user == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }
        
        bool verify = BCrypt.Net.BCrypt.Verify(testingNewPassword, user.Password);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
        verify.ShouldBe(false);
    }
    [Fact]
    public async Task UpdatePasswordCommand_UserNotExists_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 1000;
        const string testingCurrentPassword = "usermocktest";
        const string testingNewPassword = "supertestpassword!@32WW";

        UpdatePasswordCommandHandler handler = new UpdatePasswordCommandHandler(_mock.Object);

        UpdatePasswordCommand command = new UpdatePasswordCommand
        {
            RequestedUserId = testingRequestedUserId,
            CurrentPassword = testingCurrentPassword,
            NewPassword = testingNewPassword,
            ConfirmNewPassword = testingNewPassword
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserNotExists);
    }
}