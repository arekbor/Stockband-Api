using Moq;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.UpdateRole;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Application.UnitTest.RepositoryMocks;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Xunit;

namespace Stockband.Application.UnitTest.Features.UserFeatures.Commands;

public class UpdateRoleCommandTest
{
    private readonly Mock<IUserRepository> _mock;

    public UpdateRoleCommandTest()
    {
        _mock = UserRepositoryMock.GetUserRepositoryMock();
    }

    [Theory]
    [InlineData(5,3,UserRole.Admin)]
    [InlineData(5,1,UserRole.Admin)]
    [InlineData(5,4,UserRole.Admin)]
    [InlineData(5,4,UserRole.User)]
    [InlineData(5,2,UserRole.User)]
    [InlineData(5,1,UserRole.User)]
    public async Task UpdateRoleCommand_ShouldBeSuccess(int testingRequestedUserId, int testingUserId, UserRole testingRole)
    {
        UpdateRoleCommandHandler handler = new UpdateRoleCommandHandler(_mock.Object);

        UpdateRoleCommand command = new UpdateRoleCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Role = testingRole
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
        
        user.Role.ShouldBe(testingRole);
        
        requestedUser.Role.ShouldBe(UserRole.Admin);
    }

    [Fact]
    public async Task UpdateRoleCommand_RequestedUserIsNotAdmin_ShouldNotBeSuccess()
    {
        int testingRequestedUserId = 4;
        int testingUserId = 1;
        UserRole testingRole = UserRole.Admin;
        
        UpdateRoleCommandHandler handler = new UpdateRoleCommandHandler(_mock.Object);

        UpdateRoleCommand command = new UpdateRoleCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Role = testingRole
        };
        
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
}