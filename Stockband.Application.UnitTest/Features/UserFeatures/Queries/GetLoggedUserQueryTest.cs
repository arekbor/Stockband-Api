using Moq;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Queries.GetLoggedUser;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.UserFeatures.Queries;

public class GetLoggedUserQueryTest
{
    private readonly Mock<IUserRepository> _mock;

    public GetLoggedUserQueryTest()
    {
        _mock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task GetLoggedUserQuery_ShouldBeSuccess()
    {
        string testingEmail = "mock2@mock.com";
        string testingPassword = "usermocktestpassword2";

        GetLoggedUserQueryHandler handler = new GetLoggedUserQueryHandler(_mock.Object);
        GetLoggedUserQuery query = new GetLoggedUserQuery
        {
            Email = testingEmail,
            Password = testingPassword
        };
        
        BaseResponse<GetLoggedUserQueryViewModel> response = await handler.Handle(query, CancellationToken.None);

        User? user = _mock.Object.GetUserByEmailAsync(testingEmail).Result;
        if (user == null)
        {
            throw new ObjectNotFound(typeof(User), testingEmail);
        }

        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);

        response.Result.Id.ShouldBe(user.Id);
        response.Result.Email.ShouldBe(user.Email);
        response.Result.Username.ShouldBe(user.Username);
        response.Result.Role.ShouldBe(user.Role);
    }
    
    [Fact]
    public async Task GetLoggedUserQuery_InvalidEmail_ShouldNotBeSuccess()
    {
        string testingEmail = "mock2mock.com";
        string testingPassword = "usermocktestpassword2";

        GetLoggedUserQueryHandler handler = new GetLoggedUserQueryHandler(_mock.Object);
        GetLoggedUserQuery query = new GetLoggedUserQuery
        {
            Email = testingEmail,
            Password = testingPassword
        };
        
        BaseResponse<GetLoggedUserQueryViewModel> response = await handler.Handle(query, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Result.ShouldBeNull();
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
    
    [Fact]
    public async Task GetLoggedUserQuery_InvalidPassword_ShouldNotBeSuccess()
    {
        string testingEmail = "mock@2mock.com";
        string testingPassword = "";

        GetLoggedUserQueryHandler handler = new GetLoggedUserQueryHandler(_mock.Object);
        GetLoggedUserQuery query = new GetLoggedUserQuery
        {
            Email = testingEmail,
            Password = testingPassword
        };
        
        BaseResponse<GetLoggedUserQueryViewModel> response = await handler.Handle(query, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Result.ShouldBeNull();
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}