using System.Net;
using Bogus;
using FizzWare.NBuilder;
using FlueFlame.Http.Modules;
using Shouldly;
using Stockband.Api.Dtos.User;
using Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Api.E2E.Controllers.UserController;

[TestFixture]
public class UserLoginTests:BaseTest
{
    private const string TestingUri = "/user/login";
    private IUserRepository _userRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }

    [Test]
    public void UserLogin_BaseResponse_WithoutErrors()
    {
        //Arrange
        string testingEmail = new Faker().Person.Email;
        const string testingPassword = "AbcDf@#!1233";
        
        UserBuilder(testingEmail, testingPassword);

        LoginUserDto dto = LoginUserDtoBuilder(testingEmail, testingPassword);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);

        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.OK);

        responseModule.AsJson.AssertThat<BaseResponse<GetLoggedUserQueryViewModel>>(response =>
        {
            response.Errors.Count.ShouldBe(0);
            response.Success.ShouldBe(true);
            response.Result.ShouldNotBeNull();
        });
    }

    [Test]
    [TestCase("test@gmail.com","test_wrong@com.pl","Addds12@3#1d","Addds12@3#1d")]
    [TestCase("test@gmail.com","test@gmail.com","Addds12@3#1d","test_wrong@3#1d")]
    public void UserLogin_WrongEmailOrPassword_BaseErrorCodeShouldBe_WrongEmailOrPasswordLogin
        (string emailCreate, string emailLogin, string passwordCreate, string passwordLogin)
    {
        //Arrange
        UserBuilder(emailCreate, passwordCreate);
        
        LoginUserDto dto = LoginUserDtoBuilder(emailLogin, passwordLogin);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        
        responseModule.AsJson.AssertThat<BaseResponse<GetLoggedUserQueryViewModel>>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Result.ShouldBeNull();
            response.Errors.First().Code.ShouldBe(BaseErrorCode.WrongEmailOrPasswordLogin);
        });
    }

    [Test]
    [TestCase("", "Test@3@#123")]
    [TestCase("test@gmail.com", "")]
    public void UserLogin_EmptyEmailOrPassword_BaseErrorCodeShouldBe_FluentValidationCode
        (string email, string password)
    {
        //Arrange
        LoginUserDto dto = LoginUserDtoBuilder(email, password);
        
        //Act
        HttpResponseModule responseModule = ActResponseModule(dto);
        
        //Assert
        responseModule.AssertStatusCode(HttpStatusCode.BadRequest);
        
        responseModule.AsJson.AssertThat<BaseResponse<GetLoggedUserQueryViewModel>>(response =>
        {
            response.Errors.Count.ShouldBe(1);
            response.Success.ShouldBe(false);
            response.Result.ShouldBeNull();
            response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
        });
    }

    private LoginUserDto LoginUserDtoBuilder(string email, string password)
    {
        return new LoginUserDto
        {
            Email = email,
            Password = password
        };
    }

    private void UserBuilder(string email, string password)
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(password);
        
        User mockUser = Builder<User>
            .CreateNew()
            .With(x => x.Deleted = false)
            .With(x => x.Password = hash)
            .With(x => x.Email = email)
            .Build();

        _userRepository.AddAsync(mockUser);
    }
    
    private HttpResponseModule ActResponseModule(LoginUserDto dto)
    {
        return HttpHost
            .Post
            .Url(TestingUri)
            .Json(dto)
            .Send()
            .Response;
    }
}