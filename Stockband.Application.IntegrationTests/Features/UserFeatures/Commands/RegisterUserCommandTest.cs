using Bogus;
using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.RegisterUser;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.UserFeatures.Commands;

public class RegisterUserCommandTest:BaseTest
{
    private IUserRepository _userRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }
    
    [Test]
    public async Task RegisterUserCommand_ResponseShouldBeSuccess()
    {
        //Assert  
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;

        const string testingPassword = "TesTPassWord!@23";

        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingPassword,
        };
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_userRepository);
        
        //Act
        IEnumerable<User> listOfUsersBeforeHandler = await _userRepository.GetAllAsync();
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        IEnumerable<User> listOfUsersAfterHandler = await _userRepository.GetAllAsync();

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        listOfUsersAfterHandler.Count().ShouldBe(listOfUsersBeforeHandler.Count()+1);
    }

    [Test]
    public async Task RegisterUserCommand_UserAlreadyExists_ResponseShouldBeNotSuccess()
    {
        //Assert  
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        const string testingPassword = "TesTPassWord!@23";

            RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingPassword,
        };

        User userForTest = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Email = testingEmail)
            .With(x => x.Username = testingUsername)
            .Build();
        await _userRepository.AddAsync(userForTest);

        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserAlreadyCreated);
    }
    
    [Test]
    public async Task RegisterUserCommand_PasswordsNotEquals_ResponseShouldBeNotSuccess()
    {
        //Assert  
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.Email;
        const string testingPassword = "TesTPassWord!@23";
        const string testingConfirmPassword = "TesTPassWord!@23abcd";

        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingConfirmPassword,
        };
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    
    [Test]
    public async Task RegisterUserCommand_InvalidEmail_ResponseShouldBeNotSuccess()
    {
        //Assert  
        string testingUsername = new Faker().Person.UserName;
        string testingEmail = new Faker().Person.UserName;
        const string testingPassword = "TesTPassWord!@23";

        RegisterUserCommand command = new RegisterUserCommand
        {
            Username = testingUsername,
            Email = testingEmail,
            Password = testingPassword,
            ConfirmPassword = testingPassword,
        };
        
        RegisterUserCommandHandler handler = new RegisterUserCommandHandler(_userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}