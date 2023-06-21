using FizzWare.NBuilder;
using NUnit.Framework;
using Shouldly;
using Stockband.Application.Features.UserFeatures.Commands.UpdateRole;
using Stockband.Application.Interfaces.Repositories;
using Stockband.Domain;
using Stockband.Domain.Common;
using Stockband.Domain.Entities;
using Stockband.Domain.Exceptions;
using Stockband.Infrastructure.Repositories;

namespace Stockband.Application.IntegrationTests.Features.UserFeatures.Commands;

public class UpdateRoleCommandTest:BaseTest
{
    private IUserRepository _userRepository = null!;
    [SetUp]
    public void SetUp()
    {
        _userRepository = new UserRepository(Context);
    }
    
    [Test]
    [TestCase(UserRole.User, UserRole.Admin)]
    [TestCase(UserRole.Admin, UserRole.User)]
    public async Task UpdateRoleCommand_ResponseShouldBeSuccess(UserRole roleToUpdate, UserRole defaultRole)
    {
        //Arrange
        const int testingRequestedUserId = 50012;
        const int testingUserId = 11123;
        
        UpdateRoleCommand command = new UpdateRoleCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Role = roleToUpdate
        };

        List<User> usersForTest = Builder<User>
            .CreateListOfSize(2)
            .TheFirst(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = UserRole.Admin)
            .With(x => x.Id = testingRequestedUserId)
            .TheNext(1)
            .With(x => x.Deleted = false)
            .With(x => x.Role = defaultRole)
            .With(x => x.Id = testingUserId)
            .Build()
            .ToList();
        await _userRepository.AddRangeAsync(usersForTest);

        UpdateRoleCommandHandler handler = new UpdateRoleCommandHandler(_userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        User? userAfterHandler = await _userRepository.GetByIdAsync(testingUserId);
        if (userAfterHandler == null)
        {
            throw new ObjectNotFound(typeof(User), testingUserId);
        }

        //Assert
        response.Success.ShouldBe(true);
        response.Errors.Count.ShouldBe(0);
        
        userAfterHandler.Role.ShouldBe(roleToUpdate);
    }

    [Test]
    public async Task UpdateRoleCommand_RequestedUserIsNotAdmin_ResponseShouldBeNotSuccess()
    {
        //Arrange
        const int testingRequestedUserId = 1250012;
        const int testingUserId = 541213;
        
        UpdateRoleCommand command = new UpdateRoleCommand
        {
            RequestedUserId = testingRequestedUserId,
            UserId = testingUserId,
            Role = UserRole.User
        };

        List<User> usersForTest = Builder<User>
            .CreateListOfSize(2)
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
        await _userRepository.AddRangeAsync(usersForTest);
        
        UpdateRoleCommandHandler handler = new UpdateRoleCommandHandler(_userRepository);
        
        //Act
        BaseResponse response = await handler.Handle(command, CancellationToken.None);
        
        //Assert
        response.Success.ShouldBe(false);
        response.Errors.Count.ShouldBe(1);
        response.Errors.First().Code.ShouldBe(BaseErrorCode.UserUnauthorizedOperation);
    }
}