using Moq;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.RegisterUser;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Xunit;

namespace Stockband.Application.UnitTest.Features.UserFeatures.Commands;

public class RegisterUserCommandTest
{
    private readonly Mock<IUserRepository> _mock;

    public RegisterUserCommandTest()
    {
        _mock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task RegisterUserCommand_ShouldBeSuccess()
    {
        const string testingUsername = "new test user";
        const string testingEmail = "testuser@test.com";
        const string testingPassword = "TesTPassWord!@23";

        List<User> userMocksBeforeCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_mock.Object);
        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingPassword,
        };

        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        List<User> userMocksAfterCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        userMocksAfterCommand.Count.ShouldBe(userMocksBeforeCommand.Count+1);
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
    }

    [Fact]
    public async Task RegisterUserCommand_UserAlreadyExists_ShouldNotBeSuccess()
    {
        const string testingUsername = "new test user";
        const string testingEmail = "mock2@mock.com";
        const string testingPassword = "TesTPassWord!@23";
        
        List<User> userMocksBeforeCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_mock.Object);
        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingPassword,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        List<User> userMocksAfterCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        userMocksAfterCommand.Count.ShouldBe(userMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserAlreadyCreated);
    }

    [Fact]
    public async Task RegisterUserCommand_PasswordNotEquals_ShouldNotBeSuccess()
    {
        const string testingUsername = "new test user";
        const string testingEmail = "testuser@test.com";
        const string testingPassword = "TesTPassWord!@23";
        const string testingConfirmPassword = "TesTPasssWord!@232";
        
        List<User> userMocksBeforeCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_mock.Object);
        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingConfirmPassword,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        List<User> userMocksAfterCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        userMocksAfterCommand.Count.ShouldBe(userMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    
    [Fact]
    public async Task RegisterUserCommand_InvalidEmail_ShouldNotBeSuccess()
    {
        const string testingUsername = "new test user";
        const string testingEmail = "testtest.com";
        const string testingPassword = "TesTPassWord!@23";

        List<User> userMocksBeforeCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_mock.Object);
        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingPassword,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        List<User> userMocksAfterCommand = _mock.Object.GetAllAsync().Result.ToList();
        
        userMocksAfterCommand.Count.ShouldBe(userMocksBeforeCommand.Count);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}