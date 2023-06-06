using Moq;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.UpdateUser;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.UserFeatures.Commands;

public class UpdateUserCommandTest
{
    private readonly Mock<IUserRepository> _mock;

    public UpdateUserCommandTest()
    {
        _mock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Fact]
    public async Task UpdateUserCommand_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 1;
        const int testingUserId = 1;
        const string testingUsername = "test username";
        const string testingEmail = "test@test.com";

        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(_mock.Object);

        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        User? user = _mock.Object.GetByIdAsync(testingUserId).Result;
        if (user == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        user.Email.ShouldBe(testingEmail);
        user.Username.ShouldBe(testingUsername);
        user.Id.ShouldBe(testingUserId);
    }
    
    [Fact]
    public async Task UpdateUserCommand_RequestedUserIsNotOwnerButIsAdmin_ShouldBeSuccess()
    {
        const int testingRequestedUserId = 5;
        const int testingUserId = 1;
        const string testingUsername = "test username";
        const string testingEmail = "test@test.com";

        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(_mock.Object);

        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        User? user = _mock.Object.GetByIdAsync(testingUserId).Result;
        if (user == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }
        
        User? requestedUser = _mock.Object.GetByIdAsync(testingRequestedUserId).Result;
        if (requestedUser == null)
        {
            throw new ObjectNotFound(typeof(User), testingRequestedUserId);
        }
        
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        user.Email.ShouldBe(testingEmail);
        user.Username.ShouldBe(testingUsername);
        user.Id.ShouldBe(testingUserId);
        
        requestedUser.Id.ShouldNotBe(testingUserId);
        requestedUser.Role.ShouldBe(UserRole.Admin);
    }
    
    [Fact]
    public async Task UpdateUserCommand_InvalidRequestedUser_ShouldNotBeSuccess()
    {
        const int testingRequestedUserId = 3;
        const int testingUserId = 1;
        const string testingUsername = "test username";
        const string testingEmail = "test@test.com";

        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(_mock.Object);

        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
    
    [Theory]
    [InlineData(
        1,
        1,
        "testing user testing user testing user testing user testing user",
        "test@test.pl")
    ]
    [InlineData(
        1,
        1,
        "testing user",
        "invalid email")
    ]
    public async Task UpdateUserCommand_InvalidData_ShouldNotBeSuccess(
        int testingRequestedUserId, int testingUserId, string testingUsername, string testingEmail)
    {
        UpdateUserCommandHandler handler = new UpdateUserCommandHandler(_mock.Object);

        UpdateUserCommand command = new UpdateUserCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Username = testingUsername,
            Email = testingEmail,
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);

        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.FluentValidationCode);
    }
}