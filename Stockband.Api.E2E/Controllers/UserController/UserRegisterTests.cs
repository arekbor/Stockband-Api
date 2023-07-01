using System.Net;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.Dtos.User;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class UserRegisterTests:BaseTest
{
    private IUserRepository _userRepository = null!;
    private const string TestingUri = "/user/register";

    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public void UserRegister_BaseResponse_Success_ShouldBeTrue()
    {
        //Arrange
        RegisterUserDto dto = RegisterUserDtoBuilder();
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);

        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(0);
            response.Success.ShouldBe(true);
        });
    }

    [Test]
    public void UserRegister_WrongEmailScheme_BaseErrorCodeShouldBe_FluentValidationCode()
    {
        //Arrange
        RegisterUserDto dto = RegisterUserDtoBuilder(email:"test#gmail.com");
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);

        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        });
    }

    [Test]
    public void UserRegister_PasswordDoNotMatch_BaseErrorCodeShouldBe_FluentValidationCode()
    {
        //Arrange
        RegisterUserDto dto = RegisterUserDtoBuilder
            (password: "Tssd123!@333DD", confirmPassword: "ssssddWWEE@@##123");
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);

        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        });
    }

    [Test]
    public async Task UserRegister_UserEmailIsAlreadyRegistered_BaseErrorCodeShouldBe_UserAlreadyCreated()
    {
        //Arrange
        const string testingEmail = "test@gmail.com";

        await UserBuilder(testingEmail);
        
        RegisterUserDto dto = RegisterUserDtoBuilder(email: testingEmail);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);

        responseModule.AsJson.AssertThat<BaseResponse>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Errors.First().Code.ShouldBe(BaseErrorCode.UserAlreadyCreated);
        });
    }
    
    private HttpResponseModule ActResponseModule(RegisterUserDto dto)
    {
        return HttpHost
            .Post
            .Url(TestingUri)
            .Json(dto)
            .Send()
            .Response;
    }
    
    private async Task UserBuilder(string email)
    {
        User userMock =  Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Email = email)
            .With(x => x.Role == UserRole.User)
            .Build();
        await _userRepository.AddAsync(userMock);
    }

    private static RegisterUserDto RegisterUserDtoBuilder
        (string username = "", string email = "", string password = "", string confirmPassword = "")
    {
        const string testingPassword = "Tdddfgfss@!223ASD";
        
        if (String.IsNullOrEmpty(username))
            username = "testUsername";
        if (String.IsNullOrEmpty(email))
            email = "test@gmail.com";
        if (String.IsNullOrEmpty(password))
            password = testingPassword;
        if (String.IsNullOrEmpty(confirmPassword))
            confirmPassword = testingPassword;

        return Builder<RegisterUserDto>
            .CreateNew()
            .With(x => x.Email = email)
            .With(x => x.Password = password)
            .With(x => x.ConfirmPassword = confirmPassword)
            .With(x => x.Username = username)
            .Build();
    }
}